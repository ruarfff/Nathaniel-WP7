using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NathanielGame
{
    class Hermes : GameCharacter
    {
        private const int MovingSheetCols = 5;
        private readonly Rectangle[] _movingFrames = new Rectangle[MovingSheetCols];
        private Texture2D _hermesMovingTexture;
        private bool _isInBuildMode;
        public bool IsInBuildMode
        {
            get { return _isInBuildMode; }
            set
            {
                isCurrentlyMoveable = !value;
                _isInBuildMode = value;
            }
        }
        public Hermes(GameplayScreen gamePlayScreen)
            : base(gamePlayScreen)
        {
        }

        public override void Initialize()
        {
            //Animation and images
            texture = ImageManager.HermesIdleSpriteSheet;
            _hermesMovingTexture = ImageManager.HermesMovingSpriteheet;
            deadTexture = ImageManager.BloodTexture;
            animates = true;
            spriteSheetCols = 8;
            spriteSheetRows = 1;
            int movingFrameWidth = _hermesMovingTexture.Width / MovingSheetCols;
            int movingFrameHeight = _hermesMovingTexture.Height;
            for (int i = 0; i < MovingSheetCols; i++)
            {
                _movingFrames[i] = new Rectangle(i * movingFrameWidth, 0, movingFrameWidth, movingFrameHeight);
            }
            
            //PrimaryWeapon
            range = 450.0f;
            visibleRange = 500.0f;
            PrimaryWeapon = new Laser(gamePlayScreen, this){LaserColour = Color.DarkViolet, LaserThickness = 4};
            //PrimaryWeapon = new Gun(gamePlayScreen, this);
            isArmed = true;
            
            //Movement and status
            IsInBuildMode = true;
            isInMotion = false;
            destination = Center;
            MaxSpeed = 40f;
            Speed = MaxSpeed;
            
            //Hit points
            maxHP = 2000;
            startingHP = maxHP;
            currentHP = maxHP;
            
            //Initialize the draw area as a proportion of the screen size
            width = (int)(gamePlayScreen.VP.Width*0.1f);
            height = (int) (gamePlayScreen.VP.Height*0.15f);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(!IsAlive)
                Die();
        }
        protected override void UpdateState()
        {
            if (!_isInBuildMode)
            {
                destination = Vector2.Distance(PlayerManager.Nathaniel.Center, Center) <= 100 ? Center : PlayerManager.Nathaniel.Center;
            }
            switch (facingDirection)
            {
                case FacingDirection.North:
                    currentFrame = isInMotion ? _movingFrames[1] : frames[0, 2];
                    break;
                case FacingDirection.South:
                    currentFrame = isInMotion ? _movingFrames[1] : frames[0, 0];
                    break;
                case FacingDirection.NorthEast:
                    currentFrame = isInMotion ? _movingFrames[3] : frames[0, 5];
                    break;
                case FacingDirection.SouthEast:
                    currentFrame = isInMotion ? _movingFrames[2] : frames[0, 4];
                    break;
                case FacingDirection.NorthWest:
                    currentFrame = isInMotion ? _movingFrames[4] : frames[0, 7];
                    break;
                case FacingDirection.SouthWest:
                    currentFrame = isInMotion ? _movingFrames[0] : frames[0, 1];
                    break;
                case FacingDirection.East:
                    currentFrame = isInMotion ? _movingFrames[3] : frames[0, 6];
                    break;
                default:
                    currentFrame = isInMotion ? _movingFrames[4] : frames[0, 3];
                    break;
            }
            base.UpdateState();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(isInMotion ? _hermesMovingTexture : texture, Body, currentFrame, Color.White);
        }

        protected override void Die()
        {
            base.Die();
            GameManager.GameOver = true;
        }

        #region Collecting Resources
        public void CollectCorpse(Corpse corpse)
        {
            corpse.IsCollected = false;
            corpse.Center = Vector2.Zero;
            Player.resources += corpse.Resources;
            corpse.Remove();
        }
        #endregion
    }
}
