using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    public delegate void DeathEventHandler(object sender, EventArgs e);
    abstract class GameCharacter : DrawableGameAgent
    {
        public event DeathEventHandler Died;
        #region Enums
        /// <summary>
        /// Represents the direction the game agent is facing.
        /// Used for aming, animation and sometimes for movement
        /// </summary>
        protected enum FacingDirection
        {
            South,
            North,
            West,
            East,
            SouthWest,
            SouthEast,
            NorthWest,
            NorthEast
        }
        #endregion
        #region personal update thread variables
        protected Thread stateThread;
        private readonly object _dataLock = new object();
        #endregion
        #region Fields
        protected float elapsed;
        public float Elapsed
        {
            get { return elapsed; }
        }
        protected float totalTime;
        #region Movement
        private PathFinder _pathFinder;
        protected Vector2 direction;
        protected FacingDirection facingDirection;
        /// <summary>
        /// Setting this to false will stop an agent from moving to a destination
        /// </summary>
        protected bool isCurrentlyMoveable;
        public bool IsCurrentlyMoveable
        {
            get { return isCurrentlyMoveable; }
            set { isCurrentlyMoveable = value; }
        }
        protected Vector2 destination;
        public Vector2 Destination
        {
            get { return destination; }
            set { destination = value; }
        }
        protected bool isInMotion;
        private Vector2 _velocity;
        /// <summary>
        /// Determines the movement speed of an agent
        /// </summary>
        public float Speed { get; set; }
        /// <summary>
        /// An agent shouldn't travel faster than this. If an effect has been used on the agents speed
        /// this value will be used to return the speed to normal
        /// </summary>
        public float MaxSpeed { get; protected set; }
        private Vector2 _currentTargetSquare;
        #endregion
        protected bool isAttacking;
        public bool IsAttacking { get; protected set; }
        public int Threat { get; set; }
        public bool IsNonCombatant { get; set; }
        public Weapon PrimaryWeapon { get; protected set; }
        protected bool isArmed;
        protected GameCharacter target;
        public virtual GameCharacter Target
        {
            get { return target; }
            set { target = value; }
        }
        private Vector2 _target;
        /// <summary>
        /// See if the agent currently has any target
        /// </summary>
        public virtual bool HasTarget
        {
            get { return target != null && target.IsAlive; }
        }
        protected int startingHP;
        public int StartingHP
        {
            get { return startingHP; }
        }
        protected int maxHP;
        public int MaxHP
        {
            get { return maxHP; }
        }
        protected int currentHP;
        public int CurrentHP
        {
            get { return currentHP; }
            set { currentHP = value; }
        }
        public bool IsAlive
        {
            get { return currentHP > 0; }
        }
        protected float range;
        public float Range
        {
            get { return range; }
        }
        protected float visibleRange;
        public float VisibleRange
        {
            get { return visibleRange; }
        }
        protected HealthBar healthbar;
        protected Texture2D deadTexture;
        #endregion

        #region Initialisation
        protected GameCharacter(GameplayScreen gamePlayScreen)
        {
            this.gamePlayScreen = gamePlayScreen;
            healthbar = new HealthBar(ImageManager.HealthBarTexture, this);
        }

        public override void Initialize()
        {
            base.Initialize();
            IsActive = true;
            _currentTargetSquare = new Vector2();
            destination = Center;
            _currentTargetSquare = TiledMap.GetSquareAtPixel(Center);
            _target = new Vector2();
            healthbar.Initialize();
            stateThread = new Thread(UpdateThread);
            stateThread.Start();
            _pathFinder = new PathFinder();
        }
        #endregion

        #region Update and Draw
        public virtual void Update(GameTime gameTime)
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                if (isArmed && PrimaryWeapon != null)
                {
                    PrimaryWeapon.Draw(spriteBatch);
                }
                healthbar.Draw(spriteBatch);
                base.Draw(spriteBatch);

            }
            else
            {
                if (deadTexture != null)
                    DrawDead(spriteBatch);
            }
        }
        #endregion

        #region state thread
        protected virtual void UpdateThread()
        {
            while (IsActive)
            {
                //lock (_dataLock)
                //{
                    if (IsAlive && !GameplayScreen.IsPaused)
                    {
                        UpdateState();
                    }
                //}

            }
        }

        protected virtual void UpdateState()
        {

            if (isArmed && PrimaryWeapon != null)
            {
                PrimaryWeapon.Update();
                if (HasTarget)
                {
                    Attack();
                }
                else
                {
                    isAttacking = false;
                }
            }
            Move();
            healthbar.Update();

            Thread.Sleep(10);
        }
        #endregion

        protected virtual void Attack()
        {
            if (Vector2.Distance(target.Center, Center) <= range)
            {
                PrimaryWeapon.Use(elapsed);
                isAttacking = true;
            }
            else
            {
                isAttacking = false;
            }

        }

        private void Move()
        {
            if (isCurrentlyMoveable && (destination != Center))
            {
                //If we have found a path to the destination tile stop pathfinding    
                if ((_currentTargetSquare == TiledMap.GetSquareAtPixel(destination)) && ReachedTargetSquare())
                {
                    isInMotion = false;
                    destination = Center;
                }
                else
                {
                    //Set in motion to animate
                    isInMotion = true;
                    //Set direction along the path
                    direction = DetermineMoveDirection();
                    direction.Normalize();
                    _velocity = direction * Speed;
                    Center += (_velocity * elapsed);
                    /*
                    if (CheckCollision())
                    {
                        //TODO: Implement something good here
                    }
                    else
                    {
                    }*/
                }
            }
            if (HasTarget && Vector2.Distance(Center, target.Center) < range * 0.5f)
            {
                _target.X = target.Center.X - Center.X;
                _target.Y = target.Center.Y - Center.Y;
                _target.Normalize();
                GetFacingDirection(_target);
            }
            else
            {
                GetFacingDirection(direction);
            }

        }

        private Vector2 DetermineMoveDirection()
        {
            if (ReachedTargetSquare() && (_currentTargetSquare != TiledMap.GetSquareAtPixel(destination)))
            {
                _currentTargetSquare = GetNewTargetSquare();
            }
            Vector2 squareCenter = TiledMap.GetSquareCenter(
                _currentTargetSquare);
            return squareCenter - Center;
        }


        protected FacingDirection GetFacingDirection(Vector2 moveVectIn)
        {
            // Convert a movement vector to face direction
            float angle = ((float)Math.Atan2(-moveVectIn.Y, -moveVectIn.X) + MathHelper.TwoPi) % MathHelper.TwoPi;
            int polarRegion = (int)Math.Round(angle * 8f / MathHelper.TwoPi) % 8;
            if (polarRegion > 7) polarRegion -= 8;
            switch (polarRegion)
            {
                case 0: facingDirection = FacingDirection.West; break;
                case 1: facingDirection = FacingDirection.NorthWest; break;
                case 2: facingDirection = FacingDirection.North; break;
                case 3: facingDirection = FacingDirection.NorthEast; break;
                case 4: facingDirection = FacingDirection.East; break;
                case 5: facingDirection = FacingDirection.SouthEast; break;
                case 6: facingDirection = FacingDirection.South; break;
                case 7: facingDirection = FacingDirection.SouthWest; break;
                default: facingDirection = FacingDirection.South; break;

            }
            return facingDirection;
        }
        //TODO: Not using this now but still need to find a good implimentation
        /*
        protected bool CheckCollision()
        {
            if (EnemyManager.Enemies.Where(t => t != this).Any(t => t.IsCircleColliding(this)))
            {
                return true;
            }
            if (PlayerManager.PlayerCharacters.Where(t => t != this).Any(t => t.IsCircleColliding(this)))
            {
                return true;
            }

            if (PlayerManager.Nathaniel.IsCircleColliding(this) && PlayerManager.Nathaniel != this)
                return true;
            if (PlayerManager.Hermes.IsCircleColliding(this) && PlayerManager.Hermes != this)
                return true;

            return TiledMap.IsCollisionTile(TiledMap.GetSquareAtPixel(Position));

        }
        protected virtual bool CheckMoveVectorCollision()
        {
            var collisionVector = Position + direction;
            var collisionPoint = new Point((int)collisionVector.X, (int)collisionVector.Y);
            if (EnemyManager.Enemies.Any(t => t.Body.Contains(collisionPoint)))
            {
                return true;
            }
            if (PlayerManager.PlayerCharacters.Any(t => t.Body.Contains(collisionPoint)))
            {
                return true;
            }
            if (PlayerManager.Nathaniel.Body.Contains(collisionPoint) ||
                        PlayerManager.Hermes.Body.Contains(collisionPoint))
                return true;

            return TiledMap.IsCollisionTile(TiledMap.GetSquareAtPixel(direction * 10));
        }
        protected void CheckPixelCollision()
        {
            var collColor = TiledMap.GetCollision(direction * 10);
            // If there's no color data, we're not colliding
            HasCollision = collColor != null && (collColor.Value.A > 0);
        }
        */
        public virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (stateThread != null)
                    {
                        IsActive = false;
                        stateThread.Join();
                        stateThread = null;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Problem disposing " + name + " " + e.Message);
            }
        }

        private bool ReachedTargetSquare()
        {
            return (
                Vector2.Distance(
                    Center,
                    TiledMap.GetSquareCenter(_currentTargetSquare))
                <= 5);
        }

        private Vector2 GetNewTargetSquare()
        {
            List<Vector2> path = _pathFinder.FindPath(
                TiledMap.GetSquareAtPixel(Center),
                TiledMap.GetSquareAtPixel(destination));

            //This happens if the player tries to move into a wall so we'll ignore that order
            if (path == null)
            {
                destination = Center;
                return TiledMap.GetSquareAtPixel(Center);
            }
            //If we only have one path left it should mean we're next to our destination
            return path.Count > 1 ? new Vector2(path[1].X, path[1].Y) : TiledMap.GetSquareAtPixel(destination);

        }

        protected virtual void Die()
        {
            Dispose(true);
            OnDead(EventArgs.Empty);
        }

        protected virtual void OnDead(EventArgs e)
        {
            if (Died != null)
                Died(this, e);
        }

        public virtual void DrawDead(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(deadTexture, Body, Color.White);
        }

    }
}
