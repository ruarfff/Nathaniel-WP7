using System;
using Microsoft.Xna.Framework.Content;
using TiledLib;

namespace NathanielGame.Levels
{
    class LevelOne : Level
    {
        protected override void LoadMap(ContentManager content)
        {
            CurrentMap = content.Load<Map>("Maps/levelone");
            if (GameplayScreen.CurrentGameToPlay != GameToPlay.LoadGame)
            {
                startingResources = 30;
                Player.lives = 3;
            }
        }
    }
}
