using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class Soldier : BadGameCharacter
    {
        public Soldier(GameplayScreen gamePlayScreen)
            : base(gamePlayScreen)
        {
        }

        public override void Initialize()
        {
            //Animation and images
            texture = ImageManager.GreenEnemyTexture;
            deadTexture = ImageManager.CorpseTexture;
            animates = true;
            spriteSheetCols = 8;
            spriteSheetRows = 2;
            animationFramesPerSecond = 4;
            
            //PrimaryWeapon
            visibleRange = 300.0f;
            range = 300.0f;
            PrimaryWeapon = new Gun(gamePlayScreen, this);
            isArmed = true;

            //Movement and status
            destination = Center;
            isCurrentlyMoveable = true;
            isInMotion = false;
            MaxSpeed = 40f;
            Speed = MaxSpeed;
            
            //Hit points
            maxHP = 200;
            startingHP = maxHP;
            currentHP = maxHP;
            killScore = 30;

            //Initialize the draw area as a proportion of the screen size.
            width = (int)(gamePlayScreen.VP.Width * 0.075f);
            height = (int)(gamePlayScreen.VP.Height * 0.15f);
            base.Initialize();
        }

        protected override void UpdateState()
        {
            if (HasTarget && !isAttacking && (Vector2.Distance(target.Center, Center) > range * 0.6f))
            {
                destination = HasCollision ? Center : target.Center;
            }
            else
            {
                destination = Center;
            }

            switch (facingDirection)
            {
                case FacingDirection.South:
                    currentFrame = isAttacking || isInMotion ? frames[1, 0] : frames[0, 0];
                    break;
                case FacingDirection.SouthWest:
                    currentFrame = isAttacking || isInMotion ? frames[1, 1] : frames[0, 1];
                    break;
                case FacingDirection.North:
                    currentFrame = isAttacking || isInMotion ? frames[1, 2] : frames[0, 2];
                    break;
                case FacingDirection.West:
                    currentFrame = isAttacking || isInMotion ? frames[1, 3] : frames[0, 3];
                    break;
                case FacingDirection.SouthEast:
                    currentFrame = isAttacking || isInMotion ? frames[1, 4] : frames[0, 4];
                    break;
                case FacingDirection.NorthEast:
                    currentFrame = isAttacking || isInMotion ? frames[1, 5] : frames[0, 5];
                    break;
                case FacingDirection.East:
                    currentFrame = isAttacking || isInMotion ? frames[1, 6] : frames[0, 6];
                    break;
                default://NorthWest
                    currentFrame = isAttacking || isInMotion ? frames[1, 7] : frames[0, 7];
                    break;
            }


            base.UpdateState();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Body, currentFrame, Color.White);
            base.Draw(spriteBatch);
        }

        protected override void Die()
        {
            var corpse = new Corpse(Center, deadTexture, 10);
            corpse.Initialize();
            EnemyManager.Corpses.Add(corpse);
            base.Die();
        }
    }
}
