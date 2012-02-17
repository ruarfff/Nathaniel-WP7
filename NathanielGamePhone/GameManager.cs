using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NathanielGame.Levels;

namespace NathanielGame
{
    static class GameManager
    {
        private static Level _currentLevel;
        public static bool GameOver { get; set; }
        public static bool LevelWon { get; set; }
        public static void LoadCurrentLevel(GameplayScreen gameplayScreen, int level, ContentManager content)
        {
            GameOver = false;
            LevelWon = false;
            switch (level)
            {
                case 0:
                    _currentLevel = new SurvivalLevel();
                    break;
                case 1:
                    _currentLevel = new LevelOne();
                    break;
                case 2:
                    _currentLevel = new LevelTwo();
                    break;
                case 3:
                    _currentLevel = new LevelThree();
                    break;
                case 4:
                    _currentLevel = new LevelFour();
                    break;
                case 5:
                    _currentLevel = new FinalLevel();
                    break;
            }
            _currentLevel.Load(gameplayScreen,content);
        }

        public static void UpdateCurrentLevel(GameTime gameTime)
        {
            if(!GameplayScreen.IsPaused)
            _currentLevel.Update(gameTime);
        }

        public static void DrawCurrentLevel(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Level.Draw(gameTime, spriteBatch);
        }

        public static void CleanUp()
        {
            PlayerManager.Nathaniel.Dispose(true);
            PlayerManager.Hermes.Dispose(true);
            foreach (var gameCharacter in PlayerManager.PlayerCharacters)
            {
                gameCharacter.Dispose(true);
            }
            PlayerManager.Dispose(true);
            foreach (var badGameCharacter in EnemyManager.Enemies)
            {
                badGameCharacter.Dispose(true);
            }
            EnemyManager.Dispose(true);
            Player.lives = 0;
            Player.resources = 0;
            Player.score = 0;
        }

    }
}
