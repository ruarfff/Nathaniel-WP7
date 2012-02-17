using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class Boss : BadGameCharacter
    {
        private const int AnimationFramesPerSecond = 4;
        
        public Boss(GameplayScreen gamePlayScreen)
            : base(gamePlayScreen)
        {
        }

        public override void Initialize()
        {

            //Animation and images
            texture = ImageManager.Boss1Texture;
            animates = true;
            spriteSheetCols = 8;
            spriteSheetRows = 8;
            deadTexture = ImageManager.CorpseTexture;

            //PrimaryWeapon
            isArmed = true;
            PrimaryWeapon = new Bow(gamePlayScreen, this);
            visibleRange = 600.0f;
            range = 500.0f;

            //Movement and status
            isCurrentlyMoveable = true;
            isInMotion = false;
            MaxSpeed = 60f;
            Speed = 60f;
            destination = Center;
            facingDirection = FacingDirection.South;
            
            //Hit points
            maxHP = 800;
            startingHP = maxHP;
            currentHP = maxHP;
            killScore = 100;
            //Initialize the draw area as a proportion of the screen size
            width = (int)(gamePlayScreen.VP.Width * 0.1f);
            height = (int)(gamePlayScreen.VP.Height * 0.15f);

            base.Initialize();
        }

        protected override void UpdateState()
        {
            if (HasTarget && (Vector2.Distance(target.Center, Center) > range * 0.3f))
            {
                destination = HasCollision ? Center : target.Center;
            }
            else
            {
                destination = Center;
                isInMotion = false;
            }
            int index = 0;
            if (isInMotion)
            {
                // Modify the index to select the current frame of the animation.
                index += (int)(totalTime * AnimationFramesPerSecond) % 4;
            }
            switch (facingDirection)
            {
                case FacingDirection.North:
                    currentFrame = isInMotion ? frames[index, 2] : frames[5, 4];
                    break;
                case FacingDirection.South:
                    currentFrame = isInMotion ? frames[index, 0] : frames[5, 0];
                    break;
                case FacingDirection.NorthEast:
                    currentFrame = isInMotion ? frames[index, 5] : frames[5, 5];
                    break;
                case FacingDirection.SouthEast:
                    currentFrame = isInMotion ? frames[index, 4] : frames[5, 7];
                    break;
                case FacingDirection.NorthWest:
                    currentFrame = isInMotion ? frames[index, 7] : frames[5, 3];
                    break;
                case FacingDirection.SouthWest:
                    currentFrame = isInMotion ? frames[index, 1] : frames[5, 1];
                    break;
                case FacingDirection.East:
                    currentFrame = isInMotion ? frames[index, 6] : frames[5, 6];
                    break;
                default:
                    currentFrame = isInMotion ? frames[index, 3] : frames[5, 2];
                    break;
            }
          
            base.UpdateState();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Body,currentFrame, Color.White);
            base.Draw(spriteBatch);
        }

        protected override void Die()
        {
            EnemyManager.NumBosses--;
            base.Die();       
        }
    }
}
