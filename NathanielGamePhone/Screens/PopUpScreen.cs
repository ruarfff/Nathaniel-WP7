using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    abstract class PopUpScreen : DrawableGameComponent
    {
        protected GameplayScreen gameplayScreen;
        private Vector2 _position;
        private Viewport _vp;
        protected Rectangle backgroundRectangle;
        public Rectangle BackgroundRectangle
        {
            get { return backgroundRectangle; }
        }
        private float _transitionAlpha;
        public static float MenuHeight
        {
            get { return _menuHeight; }
        }
        private static float _menuHeight;
        public static float MenuWidth
        {
            get { return _menuWidth; }
        }

        private static float _menuWidth;
        
        protected static float margin;

        private Color _color;
        protected bool showing;


        protected PopUpScreen(Game game, GameplayScreen gameplayScreen)
            : base(game)
        {
            this.gameplayScreen = gameplayScreen;
        }
        public override void Initialize()
        {
            _vp = gameplayScreen.ScreenManager.GraphicsDevice.Viewport;
            _menuWidth = _vp.Width * 0.6f;
            _menuHeight = _vp.Height * 0.6f;
            margin = _menuWidth*0.2f;
            _position = new Vector2(_vp.X + _vp.Width * 0.2f, _vp.Y + _vp.Height * 0.2f);
            backgroundRectangle = new Rectangle((int)_position.X, (int)_position.Y, (int)_menuWidth, (int)_menuHeight);
            _transitionAlpha = 0.5f;
            base.Initialize();
        }

        public virtual void DrawMenu(SpriteBatch spriteBatch)
        {
           // Fade the popup alpha during transitions.
            _color = Color.White * _transitionAlpha;
            // Draw the background rectangle.
            spriteBatch.Draw(ImageManager.GradientTexture, backgroundRectangle, _color);
        }

        public void HandleInput(Point screenInputTouchPosition)
        {
            if(showing)
            {
                HandleTouchInput(screenInputTouchPosition);
            }
        }

        protected virtual void HandleTouchInput(Point screenInputPosition)
        {}

        public void Show()
        {
            showing = true;
            gameplayScreen.PauseCurrentGame(); 
        }

        public void Remove()
        {
            showing = false;
            gameplayScreen.ResumeCurrentGame();
        }
    }
}
