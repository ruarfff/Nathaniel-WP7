using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TiledLib;

namespace NathanielGame.Levels
{
    class LevelFour : Level
    {
        private double _lastSpawnCounter;
        private float _minSpawnTime;
        readonly Random _random = new Random();
        private int _index;
        private int _currentMaxIndex;

        protected override void LoadMap(ContentManager content)
        {
            CurrentMap = content.Load<Map>("Maps/survivalmap");
            startingResources = 0;
            _index = 0;
            _currentMaxIndex = 2;
            _lastSpawnCounter = 0f;
            _minSpawnTime = 5.0f;
        }

        public override void Update(GameTime gameTime)
        {
            _lastSpawnCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (_minSpawnTime > 1)
            {
                if (gameTime.TotalGameTime.TotalMinutes > 1)
                {
                    _currentMaxIndex = 3;
                    _minSpawnTime = 4;
                }
                if (gameTime.TotalGameTime.TotalMinutes > 2)
                {
                    _currentMaxIndex = 4;
                    _minSpawnTime = 3;
                }
                if (gameTime.TotalGameTime.TotalMinutes > 3)
                {
                    _currentMaxIndex = 5;
                    _minSpawnTime = 2;
                }
                if (gameTime.TotalGameTime.TotalMinutes > 4)
                {
                    _currentMaxIndex = 6;
                    _minSpawnTime = 1;
                }
                if (gameTime.TotalGameTime.TotalMinutes > 5)
                {
                    _currentMaxIndex = 7;
                }
            }
            SpawnEnemies();
            base.Update(gameTime);
        }

        private void SpawnEnemies()
        {

            if (_lastSpawnCounter > _minSpawnTime)
            {
                _index = _random.Next(0, _currentMaxIndex);
                switch (_index)
                {
                    case 1:
                        EnemyManager.AddEnemy(gameplayScreen, "Soldier",
                                      (new Vector2(_random.Next(0, CurrentMap.Width * CurrentMap.TileWidth), CurrentMap.Height * CurrentMap.TileHeight - 50)), PlayerManager.Nathaniel);
                        break;
                    case 2:
                        EnemyManager.AddEnemy(gameplayScreen, "Spawner",
                                      (new Vector2((CurrentMap.Width * CurrentMap.TileWidth) / 4f, CurrentMap.Height * CurrentMap.TileHeight - 200)), PlayerManager.Nathaniel);

                        break;
                    case 3:
                        EnemyManager.AddEnemy(gameplayScreen, "Boss",
                                      (new Vector2(_random.Next(0, CurrentMap.Width * CurrentMap.TileWidth), CurrentMap.Height * CurrentMap.TileHeight - 50)));

                        break;
                    case 4:
                        EnemyManager.AddEnemy(gameplayScreen, "Soldier",
                                      (new Vector2(_random.Next(0, CurrentMap.Width * CurrentMap.TileWidth), CurrentMap.Height * CurrentMap.TileHeight - 50)));

                        break;
                    case 5:
                        EnemyManager.AddEnemy(gameplayScreen, "Soldier",
                                      (new Vector2(_random.Next(0, CurrentMap.Width * CurrentMap.TileWidth), CurrentMap.Height * CurrentMap.TileHeight - 50)));

                        break;
                    case 6:
                        EnemyManager.AddEnemy(gameplayScreen, "Soldier",
                                      (new Vector2(_random.Next(0, CurrentMap.Width * CurrentMap.TileWidth), CurrentMap.Height * CurrentMap.TileHeight - 50)));

                        break;
                    case 7:
                        EnemyManager.AddEnemy(gameplayScreen, "Soldier",
                                      (new Vector2(_random.Next(0, CurrentMap.Width * CurrentMap.TileWidth), CurrentMap.Height * CurrentMap.TileHeight - 50)));

                        break;
                    default:
                        EnemyManager.AddEnemy(gameplayScreen, "Grunt",
                                      (new Vector2(_random.Next(0, CurrentMap.Width * CurrentMap.TileWidth), CurrentMap.Height * CurrentMap.TileHeight - 10)), PlayerManager.Nathaniel);
                        break;
                }
                _lastSpawnCounter = 0;
            }

        }
    }
}
