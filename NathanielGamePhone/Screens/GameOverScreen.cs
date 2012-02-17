using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class GameOverScreen: DrawableGameComponent
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

        private static Rectangle _playAgainMenuButton;
        private static Rectangle _exitMenuButton;
        private static Rectangle _uploadScoresButton;

        public GameOverScreen(Game game, GameplayScreen gameplayScreen)
            : base(game)
        {
            _gameplayScreen = gameplayScreen;
            _vp = _gameplayScreen.ScreenManager.GraphicsDevice.Viewport;
        }
        public override void Initialize()
        {
            _menuWidth = _vp.Width * 0.75f;
            _menuHeight = _vp.Height * 0.6f;
            _margin = _menuWidth*0.2f;
            _position = new Vector2(_vp.X + _vp.Width * 0.125f, _vp.Y + _vp.Height * 0.2f);
            _backgroundRectangle = new Rectangle((int)_position.X, (int)_position.Y, (int)_menuWidth, (int)_menuHeight);
            _transitionAlpha = 0.8f;
            _playAgainMenuButton = new Rectangle((int)(_position.X), (int)(_position.Y+_menuHeight/2), (int)(_backgroundRectangle.Width * 0.4f), (int)(_backgroundRectangle.Height * 0.4f));
            _exitMenuButton = new Rectangle((int)(_position.X + _backgroundRectangle.Width * 0.6f), (int)(_position.Y + _menuHeight / 2), (int)(_backgroundRectangle.Width * 0.4f), (int)(_backgroundRectangle.Height * 0.4f));
            if(GameplayScreen.CurrentGameToPlay==GameToPlay.SurvivalGame)
            _uploadScoresButton = new Rectangle((int)(_position.X+_menuWidth*0.3f), (int)_position.Y +20, (int)(_menuWidth*0.4f), (int)(_menuHeight*0.4f) );
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
                GameplayScreen.CurrentGameToPlay == GameToPlay.SurvivalGame
                    ? ImageManager.SurvivalGameOverImage
                    : ImageManager.GameOverImage, _backgroundRectangle, _color);

            base.Draw(gameTime);
        }

        public void HandleInput(Point screenInputTouchPosition)
        {
            try
            {
                if(_exitMenuButton.Contains(screenInputTouchPosition))
                {
                    _gameplayScreen.BackToMenu();
                }
                else if (_playAgainMenuButton.Contains(screenInputTouchPosition))
                {
                    GameManager.CleanUp();
                    _gameplayScreen.LoadContent();
                }
                else
                {
                    if (GameplayScreen.CurrentGameToPlay == GameToPlay.SurvivalGame)
                    {
                        if(_uploadScoresButton.Contains(screenInputTouchPosition))
                        {
                            _gameplayScreen.ToHighScoresScreen();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Problem at GameOver menu selection" + e.Message);
            }
        }
}
}
