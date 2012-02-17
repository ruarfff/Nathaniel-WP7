using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NathanielGame.Levels;

namespace NathanielGame
{
    class Nathaniel : GameCharacter
    {
        public bool HasCorpse { get; set; }
        public Nathaniel(GameplayScreen gamePlayScreen)
            : base(gamePlayScreen)
        {
            
        }

        public override void Initialize()
        {   
            //Animation and images
            animates = true;
            spriteSheetCols = 8;
            spriteSheetRows = 2;
            texture = ImageManager.NathanielTexture;
            deadTexture = ImageManager.BloodTexture;

            //PrimaryWeapon
            PrimaryWeapon = new Gun(gamePlayScreen, this);
            isArmed = true;
            visibleRange = 500;
            range = 450;

            //Movement and status
            MaxSpeed = 70f;
            Speed = MaxSpeed;
            isCurrentlyMoveable = true;
            isInMotion = false;
            destination = Center;
            facingDirection = FacingDirection.South;

            //Hit points
            maxHP = 8000;
            startingHP = maxHP;
            currentHP = maxHP;
            
            //Initialize the draw area as a proportion of the screen size
            width = (int)(gamePlayScreen.VP.Width * 0.06);
            height = (int)(gamePlayScreen.VP.Height * 0.15f);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!IsAlive)
            {
                if (Player.lives > 0)
                {
                    Player.lives--;
                    Respawn();
                }
                else
                {
                    Die();
                }   
            }
        }

        protected override void UpdateState()
        {
            switch (facingDirection)
            {
                case FacingDirection.South:
                    currentFrame = !isInMotion ? frames[0, 0] : frames[1, 0];
                    break;
                case FacingDirection.SouthWest:
                    currentFrame = !isInMotion ? frames[0, 1] : frames[1, 1];
                    break;
                case FacingDirection.North:
                    currentFrame = !isInMotion ? frames[0, 2] : frames[1, 2];
                    break;
                case FacingDirection.West:
                    currentFrame = !isInMotion ? frames[0, 3] : frames[1, 3];
                    break;
                case FacingDirection.SouthEast:
                    currentFrame = !isInMotion ? frames[0, 4] : frames[1, 4];
                    break;
                case FacingDirection.NorthEast:
                    currentFrame = !isInMotion ? frames[0, 5] : frames[1, 5];
                    break;
                case FacingDirection.East:
                    currentFrame = !isInMotion ? frames[0, 6] : frames[1, 6];
                    break;
                default: //NorthWest
                    currentFrame = !isInMotion ? frames[0, 7] : frames[1, 7];
                    break;
            }
            base.UpdateState();
        }
         public override void Draw(SpriteBatch spriteBatch)
         {
             base.Draw(spriteBatch);
             if (IsAlive)
             {
                 spriteBatch.Draw(texture,Body,currentFrame,Color.White);
             }
         }

        public void CollectCorpse(Corpse corpse)
        {
            corpse.Center = Center;
            corpse.IsCollected = true;
        }

        private void Respawn()
        {
            Dispose(true);
            Center = Level.StartPosition;
            target = null;
            Initialize();
        }

        protected override void Die()
        {
            base.Die();
            GameManager.GameOver = true;
        }
    }
}
