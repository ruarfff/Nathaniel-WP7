using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    static class HUD
    {
        private static GameplayScreen _gameplayScreen;
        
        #region HUD layout
        //Here some variables are declared to hold screen positions for HUD elements
        //There will be four sections at the top of the screen where elements can be placed 
        //and four at the bottom.
        private static Vector2 _topLeft;
        private static Vector2 _topCenterLeft;
        private static Vector2 _topCenterRight;
        private static Vector2 _topRight;
        private static Vector2 _bottomLeft;
        private static Vector2 _bottomCenterLeft;
        private static Vector2 _bottomCenterRight;
        private static Vector2 _bottomRight;
        private static Vector2 _livesNumberPosition;
        private static Vector2 _recourcesTextPosition;
        private static Vector2 _timeNumberPosition;
        private static Color _hermesIconColour;
        private static Color _nathanielIconColour;
        private static Color _hudTextColour;
        private static Rectangle _backDrop;
        private static Rectangle _heartIconArea;
        private static Rectangle _clockIconArea;

        public static Rectangle MenuButton { get; private set; }

        public static Rectangle NathanielButton { get; private set; }

        public static Rectangle HermesButton { get; private set; }

        public static Rectangle HermesModeButton { get; private set; }

        #endregion

        private static TimeSpan _time;
        private static string _displayTime;

        public static void Initialize(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
            var vp = _gameplayScreen.ScreenManager.GraphicsDevice.Viewport;     
            
            _displayTime = "";

            var sideMargin = vp.Width * 0.01f;
            var topMargin = vp.Height * 0.01f;
            var bottomMargin = vp.Height * 0.1f;

            _topLeft = new Vector2(vp.X + sideMargin, vp.Y + topMargin);
            _topCenterLeft = new Vector2(vp.X + vp.Width / 4 + sideMargin, vp.Y + topMargin);
            _topCenterRight = new Vector2(vp.X + vp.Width / 2 + sideMargin, topMargin);
            _topRight = new Vector2(vp.X + vp.Width * 0.75f + sideMargin, vp.Y + topMargin);

            _bottomLeft = new Vector2(vp.X + sideMargin, vp.Height - bottomMargin);
            _bottomCenterLeft = new Vector2(vp.X + vp.Width / 4 + sideMargin, vp.Height - bottomMargin);
            _bottomCenterRight = new Vector2(vp.X + vp.Width / 2 + sideMargin, vp.Height - bottomMargin);
            _bottomRight = new Vector2(vp.X + vp.Width * 0.75f + sideMargin, vp.Height - bottomMargin);

            _hudTextColour = Color.Bisque;
            _nathanielIconColour = Color.White;
            _hermesIconColour = Color.White;

            var backdropHeight = (int)bottomMargin;
            var iconWidth = (int)(vp.Width * 0.125f);
            var iconHeight = (int)(backdropHeight * 0.75f);
            const int spacing = 10;

            _backDrop = new Rectangle(0, 0, vp.Width, backdropHeight);
            //Populate the top part of the HUD
            HermesButton = new Rectangle((int)_topLeft.X, (int)_topLeft.Y, iconWidth, iconHeight);
            HermesModeButton = new Rectangle((int)_topLeft.X, HermesButton.Bottom, iconWidth, iconHeight);
            NathanielButton = new Rectangle((int)_topLeft.X + iconWidth + spacing, (int)_topLeft.Y, iconWidth, iconHeight);
            _heartIconArea = new Rectangle(NathanielButton.Right+spacing, (int)_topLeft.Y+spacing, iconWidth-spacing*2, iconHeight-spacing);
            _livesNumberPosition = new Vector2(_heartIconArea.Center.X - 11, _heartIconArea.Center.Y - 22);
            _recourcesTextPosition = new Vector2(_heartIconArea.Right+spacing, _topRight.Y);
            _clockIconArea = new Rectangle(_heartIconArea.Right+spacing*20, (int)_topRight.Y, 30,30);
            _timeNumberPosition = new Vector2(_clockIconArea.Right+spacing, _topRight.Y);
            MenuButton = new Rectangle((int)_topRight.X+spacing*4, (int)_topRight.Y, (int)(iconWidth*1.25f), iconHeight);
        }   

        public static bool HandleInput(Point touchInputLocation)
        {
            if (NathanielButton.Contains(touchInputLocation))
            {
                BuildStructureMenu.IsActive = false;
                GameplayScreen.FocusedAgent = PlayerManager.Nathaniel;
                return true;
            }
            if (HermesButton.Contains(touchInputLocation))
            {
                BuildStructureMenu.IsActive = false;
                GameplayScreen.FocusedAgent = PlayerManager.Hermes;
                return true;
            }
            if (HermesModeButton.Contains(touchInputLocation) && GameplayScreen.FocusedAgent.Equals(PlayerManager.Hermes))
            {
                PlayerManager.Hermes.IsInBuildMode = !PlayerManager.Hermes.IsInBuildMode;
                if(!PlayerManager.Hermes.IsInBuildMode)
                {
                    GameplayScreen.FocusedAgent = PlayerManager.Nathaniel;
                }
                return true;
            }
            if(MenuButton.Contains(touchInputLocation))
            {
                if (GameplayScreen.IsPaused)
                {
                    GameplayScreen.GamePauseMenu.Remove();
                    _gameplayScreen.ResumeCurrentGame();             
                }
                else
                {
                        GameplayScreen.GamePauseMenu.Show(); 
                }


            }
            return _backDrop.Contains(touchInputLocation);
        }

        public static void Update(GameTime gameTime)
        {
            _time = LevelTime.CurrentTime;
            
            if (_time.Minutes < 10)
                _displayTime = "0" + _time.Minutes;
            else
                _displayTime = "" + _time.Minutes;
            if (_time.Seconds < 10)
                _displayTime += ":0" + _time.Seconds;
            else
                _displayTime += ":" + _time.Seconds;
 
            if(GameplayScreen.FocusedAgent == PlayerManager.Nathaniel)
            {
                _nathanielIconColour = Color.White*2f;
                _hermesIconColour = Color.White*0.4f;
            }
            else
            {
                _nathanielIconColour = Color.White * 0.4f;
                _hermesIconColour = Color.White * 2f;
                
            }
        }
       
        public static void Draw(SpriteBatch spriteBatch, SpriteFont font, DrawableGameAgent focusedAgent, double totalTime)
        {
            spriteBatch.Draw(ImageManager.GradientTexture, _backDrop, Color.White * 0.5f);
            spriteBatch.Draw(ImageManager.NathanielIcon, NathanielButton, _nathanielIconColour);
            spriteBatch.Draw(ImageManager.HermesIcon, HermesButton, _hermesIconColour);
            spriteBatch.Draw(ImageManager.HeartIcon, _heartIconArea, Color.White);
            spriteBatch.DrawString(ImageManager.HudFont, Player.lives.ToString(), _livesNumberPosition, _hudTextColour);
            spriteBatch.DrawString(ImageManager.HudFont, "Resources " + Player.resources, _recourcesTextPosition, Color.Black);
            spriteBatch.DrawString(ImageManager.HudFont, "Resources " + Player.resources, _recourcesTextPosition, _hudTextColour);

            spriteBatch.Draw(ImageManager.ClockTexture, _clockIconArea, Color.White);

            spriteBatch.DrawString(ImageManager.HudFont, _displayTime, _timeNumberPosition, Color.Black);
            spriteBatch.DrawString(ImageManager.HudFont, _displayTime, _timeNumberPosition, _hudTextColour);

            spriteBatch.Draw(ImageManager.MenuButtonIcon, MenuButton, Color.White);

            if (focusedAgent.Equals(PlayerManager.Hermes))
            {
                spriteBatch.Draw(PlayerManager.Hermes.IsInBuildMode ? ImageManager.MoveModeIcon : ImageManager.BuildModeIcon, HermesModeButton,
                                 Color.White * 0.6f);
            }
            spriteBatch.DrawString(ImageManager.HudFont, "Score: " + Player.score, _bottomRight, Color.Bisque);
        }
    }
}
