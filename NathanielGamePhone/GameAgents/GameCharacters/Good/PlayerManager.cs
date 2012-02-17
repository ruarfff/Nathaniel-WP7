using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    static class PlayerManager
    {
 
        public static List<GameCharacter> PlayerCharacters { get; private set; }

        private static Nathaniel _nathaniel;
        public static Nathaniel Nathaniel
        {
            get { return _nathaniel; }
        }
        private static Hermes _hermes;
        public static Hermes Hermes
        {
            get { return _hermes; }
        }

       

        public static BuildStructureMenu BuildMenu { get; private set; }

        public static void Initialize(Game game, GameplayScreen gamePlayScreen, Nathaniel nathaniel, Hermes hermes)
        {
            PlayerCharacters = new List<GameCharacter>();
            _nathaniel = nathaniel;
            _hermes = hermes;
            _nathaniel.Initialize();
            _hermes.Initialize();
            PlayerCharacters.Add(_nathaniel);
            PlayerCharacters.Add(_hermes);
            GameplayScreen.FocusedAgent = Nathaniel;
            BuildMenu = new BuildStructureMenu(game, gamePlayScreen);
            BuildMenu.Initialize();
        }

        public static bool HandleInput(Point worldTouchInputLocation, Point screenTouchInputLocation)
        {
            if (!GameplayScreen.IsPaused)
            {
                if (BuildStructureMenu.IsActive)
                {
                    if (!(BuildMenu.BackgroundRectangle.Contains(screenTouchInputLocation)))
                    {
                        BuildStructureMenu.IsActive = false;
                        return true;
                    }
                    if ((BuildMenu.BackgroundRectangle.Contains(screenTouchInputLocation)))
                    {
                        BuildStructureMenu.IsActive = true;
                        return true;
                    }
                }
                else
                {
                    if (Nathaniel.Body.Contains(worldTouchInputLocation))
                    {
                        GameplayScreen.FocusedAgent = Nathaniel;
                    }
                    else if (Hermes.Body.Contains(worldTouchInputLocation))
                    {
                        if (Hermes.IsInBuildMode)
                        {
                            if (!(GameplayScreen.FocusedAgent.Equals(Nathaniel) && Nathaniel.HasCorpse))
                            {
                                BuildStructureMenu.IsActive = true;
                                Debug.WriteLine(BuildStructureMenu.IsActive);
                                return true;
                            }
                        }
                        else
                        {
                            GameplayScreen.FocusedAgent = Hermes;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void Update(GameTime gameTime)
        {
            try
            {
                if (!GameplayScreen.IsPaused)
                {
                    // if (BuildStructureMenu.IsActive)
                    BuildMenu.Update(gameTime);

                    if (EnemyManager.Enemies.Count <= 0)
                    {
                        Nathaniel.Update(gameTime);
                        Hermes.Update(gameTime);
                    }

                    for (int j = 0; j < EnemyManager.Enemies.Count; j++)
                    {
                        for (int i = PlayerCharacters.Count - 1; i >= 0; i--)
                        {
                            
                            var playerChar = PlayerCharacters[i];
                            playerChar.Update(gameTime);
                            if (!playerChar.IsAlive)
                            {
                                PlayerCharacters.RemoveAt(i);
                            }
                            
                            if (playerChar == GameplayScreen.FocusedAgent)
                            {
                                if (EnemyManager.Enemies[j].Target != null &&
                                    (Vector2.Distance(EnemyManager.Enemies[i].Center, playerChar.Center) <=
                                     playerChar.Range) && !playerChar.HasTarget)
                                {
                                    playerChar.Target = EnemyManager.Enemies[i];
                                }
                                continue;
                            }
                            if (!playerChar.HasTarget)
                            {
                                if (Vector2.Distance(playerChar.Center, EnemyManager.Enemies[j].Center) <
                                    playerChar.VisibleRange)
                                {
                                    playerChar.Target = EnemyManager.Enemies[j];
                                }
                                else if (EnemyManager.Enemies[j].Target != null &&
                                         EnemyManager.Enemies[j].Target.Equals(playerChar))
                                {
                                    playerChar.Target = EnemyManager.Enemies[j];
                                }

                            }
                            if ((playerChar.Target != null && playerChar.Target.IsAlive) &&
                                     Vector2.Distance(EnemyManager.Enemies[i].Center, playerChar.Center) <
                                     Vector2.Distance(playerChar.Center, playerChar.Target.Center))
                            {
                                playerChar.Target = EnemyManager.Enemies[i];
                            }
                            

                            
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int count = PlayerCharacters.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                PlayerCharacters[i].Draw(spriteBatch);
            }
        }

        public static void Dispose(bool disposing)
        {
            PlayerCharacters.Clear();
        }
    }
}
