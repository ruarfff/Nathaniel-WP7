using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace NathanielGame.Levels
{
    abstract class Level
    {
        public static Map CurrentMap { get; protected set; }
        protected GameplayScreen gameplayScreen;
        protected static int startingResources;
        protected static double elapsedTime;
        protected static double totalTime;

        public static Vector2 StartPosition { get; protected set; }
        public static Vector2 TriggerA { get; protected set; }
        public static Vector2 TriggerB { get; protected set; }
        public static Vector2 TriggerC { get; protected set; }

        Nathaniel _nathaniel;
        Hermes _hermes;

        public void Load(GameplayScreen currentGameplayScreen, ContentManager content)
        {
            LevelTime.Initialize();
            totalTime = 0;
            gameplayScreen = currentGameplayScreen;
            LoadAssets(content);
            Player.resources = startingResources;
            Player.score = 0;
            if (_nathaniel != null && _hermes != null)
            {
                PlayerManager.Initialize(gameplayScreen.ScreenManager.Game, gameplayScreen, _nathaniel, _hermes);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!GameManager.LevelWon)
            {
                elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;
                totalTime += elapsedTime;
                UpdateGameComponents(gameTime);
                UpdateGameState(gameTime);
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!GameManager.LevelWon)
            {
                DrawGameComponents(gameTime, spriteBatch);
            }
        }

        protected abstract void LoadMap(ContentManager content);

        private void LoadAssets(ContentManager content)
        {
            LoadMap(content);
            TiledMap.Initialize();
            EnemyManager.Initialize();
            
            var objects = CurrentMap.GetLayer("Objects") as MapObjectLayer;
            if (objects == null) return;
           // if(GameplayScreen.CurrentGameToPlay != GameToPlay.LoadGame){
                for (int i = 0; i < objects.Objects.Count; i++)
                {
                    MapObject mo = objects.Objects[i];
                    switch (mo.Type)
                    {
                        case "Enemy":
                            EnemyManager.AddEnemy(gameplayScreen, mo.Name,
                                                  new Vector2(mo.Bounds.X, mo.Bounds.Y));
                            break;
                        case "Nathaniel":
                            _nathaniel = new Nathaniel(gameplayScreen) { Center = new Vector2(mo.Bounds.X, mo.Bounds.Y), Name = mo.Name };
                            StartPosition = _nathaniel.Center;
                            break;
                        case "Hermes":
                            _hermes = new Hermes(gameplayScreen) { Center = new Vector2(mo.Bounds.X, mo.Bounds.Y), Name = mo.Name };
                            break;
                        case "TriggerA":
                            TriggerA = new Vector2(mo.Bounds.X, mo.Bounds.Y);
                            break;
                        case "TriggerB":
                            TriggerB = new Vector2(mo.Bounds.X, mo.Bounds.Y);
                            break;
                        case "TriggerC":
                            TriggerC = new Vector2(mo.Bounds.X, mo.Bounds.Y);
                            break;
                    }
               // }
            }
        }

        private static void UpdateGameComponents(GameTime gameTime)
        {
            // Update the player characters
            PlayerManager.Update(gameTime);
            //Update all the other characters
            EnemyManager.Update(gameTime);
        }

        private static void UpdateGameState(GameTime gameTime)
        {
            var n = PlayerManager.Nathaniel;
            var h = PlayerManager.Hermes;


            if (EnemyManager.Corpses.Count > 0)
                foreach (var corpse in EnemyManager.Corpses.Where(corpse => corpse != null))
                {
                    if (n.Body.Intersects(corpse.Body))
                    {
                        n.CollectCorpse(corpse);
                        n.HasCorpse = true;
                    }
                    if (h.Body.Intersects(corpse.Body))
                    {
                        h.CollectCorpse(corpse);
                        n.HasCorpse = false;
                        break;
                    }
                    corpse.Update(gameTime);
                    if (corpse.Removed) break;
                }
            else
                n.HasCorpse = false;
        }

        private static void DrawGameComponents(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // create a matrix for the camera to offset everything we draw, the map and our objects. since the
            // camera coordinates are where the camera is, we offset everything by the negative of that to simulate
            // a camera moving. we also cast to integers to avoid filtering artifacts
            Matrix cameraMatrix = Matrix.CreateTranslation(-(int)GameplayScreen.Camera.Position.X, -(int)GameplayScreen.Camera.Position.Y, 0);
            // Using 0 and null to indicate default values used
            //Camera matrix is used to offset anything drawn in this spritebatch call
            spriteBatch.Begin(0, null, null, null, null, null, cameraMatrix);

            // Draw the visible area of the map
            CurrentMap.Draw(spriteBatch, Camera.VisibleArea);
            AnimationManager.Draw(spriteBatch);
            EnemyManager.Draw(gameTime, spriteBatch);
            PlayerManager.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

    }
}
