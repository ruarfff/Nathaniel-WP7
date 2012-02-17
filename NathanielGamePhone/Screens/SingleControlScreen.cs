using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NathanielGame
{
    class SingleControlScreen :GameScreen
    {
        /// <summary>
        /// The sole Control in this screen. Derived classes can do what they like with it.
        /// </summary>
        protected Control RootControl;

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            if (RootControl != null)
            {
                Control.BatchDraw(RootControl, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            RootControl.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(InputState input)
        {
            // cancel the current screen if the user presses the back button
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, null, out player))
            {
                ExitScreen();
            }

            RootControl.HandleInput(input);

            base.HandleInput(input);
        }
    }
}
