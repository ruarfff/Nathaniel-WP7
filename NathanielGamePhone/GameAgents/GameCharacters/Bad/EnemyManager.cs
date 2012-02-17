using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    static class EnemyManager
    {
        private static Boss _boss;
        private static readonly List<Corpse> _corpses = new List<Corpse>();
        public static List<Corpse> Corpses
        {
            get { return _corpses; }
        }
        private static readonly List<BadGameCharacter> _enemies = new List<BadGameCharacter>();
        public static List<BadGameCharacter> Enemies
        {
            get { return _enemies; }
        }

        public static int NumSpawners { get; set; }
        public static int NumSoldiers { get; set; }
        public static int NumBosses { get; set; }

        

        public static void Initialize()
        {
            NumSoldiers = 0;
            NumSpawners = 0;
            NumBosses = 0;
        }
        public static void AddEnemy(GameplayScreen gameplayScreen, string name, Vector2 position)
        {
               AddEnemy(gameplayScreen, name, position, null);
        }

        public static void AddEnemy(GameplayScreen gameplayScreen, string name, Vector2 position, GameCharacter target)
        {
            if(name.StartsWith("Gr"))
            {
                var g = new Grunt(gameplayScreen){Center = position};
                g.Initialize();
                g.Name = name;
                if(target != null)
                g.Target = target;
                _enemies.Add(g);
            }
            else if(name.StartsWith("So"))
            {
                var m = new Soldier(gameplayScreen) { Center = position };
                m.Initialize();
                m.Name = name;
                m.Target = target;
                _enemies.Add(m);
                NumSoldiers += 1;
            }
            else if(name.StartsWith("Sp"))
            {
                var s = new Spawner(gameplayScreen) { Center = position };
                s.Initialize();
                s.Name = name;
                s.Target = target;
                _enemies.Add(s);
            }
            else if(name.StartsWith("Bo"))
            {
                _boss = new Boss(gameplayScreen) { Center = position };
                _boss.Initialize();
                _boss.Name = name;
                NumBosses++;
                _boss.Target = target;
                _enemies.Add(_boss);
                _boss.Died += new DeathEventHandler(_boss_Died);
            }
        }

        static void _boss_Died(object sender, EventArgs e)
        {
            if(GameplayScreen.CurrentGameToPlay != GameToPlay.SurvivalGame)
            GameManager.LevelWon = true;
        }

        public static bool HandleInput(Point touchInputPosition)
        {
            if (!GameplayScreen.IsPaused)
            {
                foreach (var e in Enemies.Where(
                    e => e.Body.Contains(touchInputPosition))
                    )
                {
                    PlayerManager.Nathaniel.Target = e;
                    return true;
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
                    for (int i = 0; i < _enemies.Count; i++)
                    {
                        var enemy = _enemies[i];
                        if (GameplayScreen.FocusedAgent.Target != null)
                            enemy.IsSelected = enemy.Equals(GameplayScreen.FocusedAgent.Target);
                        if (!enemy.HasTarget)
                        {
                            if (enemy.IsSelected)
                            {
                                enemy.Target = GameplayScreen.FocusedAgent;
                            }
                            else
                            {
                                for (int j = 0; j < PlayerManager.PlayerCharacters.Count; j++)
                                {
                                    var playerEntity = PlayerManager.PlayerCharacters[j];
                                    if (Vector2.Distance(enemy.Center, playerEntity.Center) < enemy.VisibleRange)
                                    {
                                        enemy.Target = playerEntity;
                                    }
                                    else if (playerEntity.HasTarget && playerEntity.Target.Equals(enemy))
                                    {
                                        enemy.Target = playerEntity;
                                    }

                                }
                            }
                        }
                        enemy.Update(gameTime);
                        if (!enemy.IsAlive)
                            Enemies.RemoveAt(i);
                    }
                    for (int i = 0; i < Corpses.Count; i++)
                    {
                        var corpse = Corpses[i];
                        corpse.Update(gameTime);
                        if (corpse.Removed) break;
                    }
                }
            }
            catch
            {
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _corpses.Count; i++)
            {
                var corpse = _corpses[i];
                corpse.Draw(spriteBatch);
            }
            for (int i = 0; i < _enemies.Count; i++)
            {
                if(Vector2.Distance(_enemies[i].Center, Camera.Center) < Camera.VisibleArea.Width)
                _enemies[i].Draw(spriteBatch);
            }
        }

        private static void UnloadContent()
        {
            foreach (var badGameCharacter in Enemies)
            {
                badGameCharacter.Dispose(true);
            }
            Enemies.Clear();
            Corpses.Clear();
        }

        public static void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {       
                    UnloadContent();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Problem disposing Enemy Manager "+ e.Message);
            }
        }

    }
}
