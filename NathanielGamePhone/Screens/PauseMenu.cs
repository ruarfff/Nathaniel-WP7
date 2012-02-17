using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class PauseMenu : PopUpScreen
    {
        
        private static Rectangle _saveMenuButton;
        private static Rectangle _resumeMenuButton;

        public PauseMenu(Game game, GameplayScreen gameplayScreen)
            : base(game, gameplayScreen)
        {
        }
        public override void Initialize()
        {
            base.Initialize();
            _saveMenuButton = new Rectangle((int)(backgroundRectangle.X+margin),backgroundRectangle.Y+40, (int)(backgroundRectangle.Width*0.6f), (int)(backgroundRectangle.Height*0.3f));
            _resumeMenuButton = new Rectangle((int)(backgroundRectangle.X + margin), _saveMenuButton.Bottom+40, (int)(backgroundRectangle.Width * 0.6f), (int)(backgroundRectangle.Height * 0.3f));
        }

        public override void DrawMenu(SpriteBatch spriteBatch)
        {
            if (showing)
            {
                base.DrawMenu(spriteBatch);
                spriteBatch.Draw(ImageManager.SaveButtonIcon, _saveMenuButton, Color.White);
                spriteBatch.Draw(ImageManager.ResumeButtonIcon, _resumeMenuButton, Color.White);
            }
        }

        protected override void HandleTouchInput(Point screenInputTouchPosition)
        {
            try
            {
                if(_resumeMenuButton.Contains(screenInputTouchPosition))
                {
                    Remove();
                    gameplayScreen.ResumeCurrentGame();
                }
                else if(_saveMenuButton.Contains(screenInputTouchPosition))
                {
                    gameplayScreen.SaveGame();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Problem at pause menu selection" + e.Message);
            }
        }

        
    }
}
