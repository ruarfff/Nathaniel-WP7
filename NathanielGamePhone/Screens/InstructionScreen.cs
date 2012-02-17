using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace NathanielGame
{
    class InstructionScreen: MenuScreen
    {
        private readonly float _marginLeft;
        private readonly PlayerIndexEventArgs _e;
        private GameToPlay _gameToPlay;
        public InstructionScreen(PlayerIndexEventArgs e, GameToPlay gameToPlay)
            :base("")
        {
            _gameToPlay = gameToPlay;
            _marginLeft = 110;
            _e = e;
        }

        public override void HandleInput(InputState input)
        {
            foreach (GestureSample gest in input.Gestures)
            {
                if(gest.GestureType == GestureType.Tap)
                {
                    if (_gameToPlay == GameToPlay.NewGame)
                    {
                        LoadingScreen.Load(ScreenManager, true, _e.PlayerIndex,
                                           new GameplayScreen(GameToPlay.NewGame));
                    }
                    else
                    {
                        LoadingScreen.Load(ScreenManager, true, _e.PlayerIndex,
                               new GameplayScreen(GameToPlay.SurvivalGame));
                    }
                }
            }
            base.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawString(ScreenManager.Font, "To move the main character touch a location ", new Vector2(_marginLeft, 100), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "on the screen.", new Vector2(_marginLeft, 130), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "To switch between characters touch ", new Vector2(_marginLeft, 160), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "the icons on the top left of the screen.", new Vector2(_marginLeft, 190), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "Collect resources by killing certain enemies ", new Vector2(_marginLeft, 220), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "and deliver the corpse they drop to Hermes.", new Vector2(_marginLeft, 250), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "To build structures touch Hermes", new Vector2(_marginLeft, 280), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "and a menu will appear.", new Vector2(_marginLeft, 310), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "Drag and drop to the desired location.", new Vector2(_marginLeft, 340), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "To win kill everything.", new Vector2(_marginLeft, 370), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "****Tap the screen to play****", new Vector2(_marginLeft, 400), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        
    }
}
