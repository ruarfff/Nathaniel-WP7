using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace NathanielGame
{
    class HighScoresScreen : SingleControlScreen
    {

        public override void LoadContent()
        {
            EnabledGestures = ScrollTracker.GesturesNeeded;
            ContentManager content = ScreenManager.Game.Content;

            RootControl = new HighScoresPanel(content);
            base.LoadContent();
        }
    }
}
