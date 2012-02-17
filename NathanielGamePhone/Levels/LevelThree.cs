using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using TiledLib;

namespace NathanielGame.Levels
{
    class LevelThree : Level
    {
        protected override void LoadMap(ContentManager content)
        {
            CurrentMap = content.Load<Map>("Maps/levelthree");
            if (GameplayScreen.CurrentGameToPlay != GameToPlay.LoadGame)
            {
                startingResources = 30;
                Player.lives = 3;
            }
        }
    }
}