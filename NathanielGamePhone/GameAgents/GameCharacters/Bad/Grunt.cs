using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class Grunt : BadGameCharacter
    {
        private Rectangle _drawArea;
        public Grunt(GameplayScreen gamePlayScreen) : base(gamePlayScreen)
        {
        }

        public override void Initialize()
        {
            //Animation and images
            texture = ImageManager.GruntTexture;
            deadTexture = ImageManager.CorpseTexture;
            animates = true;
            spriteSheetCols = 8;
            spriteSheetRows = 4;
            animationFramesPerSecond = 4;

            //PrimaryWeapon
            visibleRange = 800.0f;
            range = 250.0f;
            PrimaryWeapon = new Blaster(gamePlayScreen, this);
            isArmed = true;

            //Movement and status
            destination = Center;
            isCurrentlyMoveable = true;
            isInMotion = false;
            MaxSpeed = 70f;
            Speed = MaxSpeed;

            //Hit points
            maxHP = 150;
            startingHP = maxHP;
            currentHP = startingHP;
            killScore = 20;
            target = PlayerManager.Nathaniel;

            //Initialize the draw area as a proportion of the screen size.
            _drawArea = new Rectangle((int)Center.X, (int)Center.Y, (int)(gamePlayScreen.VP.Width * 0.09f), (int)(gamePlayScreen.VP.Height * 0.07f));
            width = (int)(gamePlayScreen.VP.Width * 0.12f);
            height = (int)(gamePlayScreen.VP.Height * 0.1f);
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
                    currentFrame = isAttacking || !isInMotion ? frames[0, 0] : frames[1, 0];
                    break;
                case FacingDirection.SouthWest:
                    currentFrame = isAttacking || !isInMotion ? frames[0, 1] : frames[1, 1];
                    break;
                case FacingDirection.North:
                    currentFrame = isAttacking || !isInMotion ? frames[0, 2] : frames[1, 2];
                    break;
                case FacingDirection.West:
                    currentFrame = isAttacking || !isInMotion ? frames[0, 3] : frames[1, 3];
                    break;
                case FacingDirection.SouthEast:
                    currentFrame = isAttacking || !isInMotion ? frames[0, 4] : frames[1, 4];
                    break;
                case FacingDirection.NorthEast:
                    currentFrame = isAttacking || !isInMotion ? frames[0, 5] : frames[1, 5];
                    break;
                case FacingDirection.East:
                    currentFrame = isAttacking || !isInMotion ? frames[0, 6] : frames[1, 6];
                    break;
                default://NorthWest
                    currentFrame = isAttacking || !isInMotion ? frames[0, 7] : frames[1, 7];
                    break;
            }
            _drawArea.X = ((int) Center.X - (_drawArea.Width/2));
            _drawArea.Y = ((int)Center.Y - (_drawArea.Height / 2));
            base.UpdateState();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, _drawArea, currentFrame, Color.White);
            base.Draw(spriteBatch);
        }

        protected override void Die()
        {
            AnimationManager.AddExplosion(Center, width, height);
            Audio.ExplosionSound();
            base.Die();
        }
    }
}
