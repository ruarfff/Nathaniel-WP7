using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class WinScreen : DrawableGameComponent
    {
        private static GameplayScreen _gameplayScreen;
        private Vector2 _position;
        private Viewport _vp;
        private Rectangle _backgroundRectangle;
        public Rectangle BackgroundRectangle
        {
            get { return _backgroundRectangle; }
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

        private static float _margin;

        private Color _color;

        public WinScreen(Game game, GameplayScreen gameplayScreen)
            : base(game)
        {
            _gameplayScreen = gameplayScreen;
            _vp = _gameplayScreen.ScreenManager.GraphicsDevice.Viewport;
        }
         
        public override void Initialize()
        {
            _menuWidth = _vp.Width * 0.75f;
            _menuHeight = _vp.Height * 0.6f;
            _margin = _menuWidth * 0.2f;
            _position = new Vector2(_vp.X + _vp.Width * 0.125f, _vp.Y + _vp.Height * 0.2f);
            _backgroundRectangle = new Rectangle((int)_position.X, (int)_position.Y, (int)_menuWidth, (int)_menuHeight);
            _transitionAlpha = 0.8f;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void DrawMenu(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Fade the popup alpha during transitions.
            _color = Color.White * _transitionAlpha;
            spriteBatch.Draw(
                ImageManager.WinScreenImage, _backgroundRectangle, _color);

            base.Draw(gameTime);
        }

        public void HandleInput(Point screenInputTouchPosition)
        {
           
                if(_backgroundRectangle.Contains(screenInputTouchPosition))
                {
                    _gameplayScreen.ToNextLevel();
                }
            
        }
    }
}
