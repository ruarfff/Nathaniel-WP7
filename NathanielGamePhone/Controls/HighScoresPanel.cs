using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class HighScoresPanel : ScrollingPanelControl
    {
        Control resultListControl = null;
        public static string Message = null;
        SpriteFont titleFont;
        SpriteFont headerFont;
        SpriteFont detailFont;
        public static bool Populated;
        private int _count = 0;
        public HighScoresPanel(ContentManager content)
        {
            titleFont = content.Load<SpriteFont>("Fonts\\menufont");
            headerFont = content.Load<SpriteFont>("Fonts\\menufont");
            detailFont = content.Load<SpriteFont>("Fonts\\menufont");
            HighScores.GetScores();
            Message = "Loading Please Wait....";
            AddChild(new TextControl("High scores", titleFont));
            AddChild(CreateHeaderControl());
            Populate();
        }


        private void Populate()
        {
            try
            {
                List<Score> highSores = new List<Score>();

                PanelControl newList = new PanelControl();
                
                if (HighScores.scores.Count > 0)
                {
                    Thread.Sleep(3000);
                    
                    highSores = HighScores.scores;
                    foreach (var s in highSores)
                    {
                        newList.AddChild(CreateLeaderboardEntryControl(s.name, s.score, s.time));
                        _count++;
                    }
                    if (_count == HighScores.scores.Count)
                    {
                        Populated = true;
                    }
                   // newList.RemoveChildAt(0);
                }
                else
                {
                    newList.AddChild(CreateLeaderboardEntryControl(Message, " ", " "));
                }
                newList.LayoutColumn(0, 0, 0);

                if (resultListControl != null)
                {
                    RemoveChild(resultListControl);
                }
                resultListControl = newList;
                AddChild(resultListControl);
                LayoutColumn(0, 0, 0);
            }
            catch
            {
                
            }
        }

        public override void Update(GameTime gametime)
        {
            if(HighScores.scores.Count > 0 && !Populated)
            {  
                Populate();  
            }
            base.Update(gametime);
        }

        protected Control CreateHeaderControl()
        {
            PanelControl panel = new PanelControl();

            panel.AddChild(new TextControl("Player", headerFont, Color.Turquoise, new Vector2(0, 0)));
            panel.AddChild(new TextControl("Score", headerFont, Color.Turquoise, new Vector2(200, 0)));

            return panel;
        }

        // Create a Control to display one entry in a leaderboard. The content is broken out into a parameter
        // list so that we can easily create a control with fake data when running under the emulator.
        //
        // Note that for time leaderboards, this function interprets the time as a count in seconds. The
        // value posted is simply a long, so your leaderboard might actually measure time in ticks, milliseconds,
        // or microfortnights. If that is the case, adjust this function to display appropriately.
        protected Control CreateLeaderboardEntryControl(string player, string score, string time)
        {
            Color textColor = Color.White;

            var panel = new PanelControl();

            // Player name
            panel.AddChild(
                new TextControl
                {
                    Text = player,
                    Font = detailFont,
                    Color = textColor,
                    Position = new Vector2(0, 0)
                });

            // Score
            panel.AddChild(
                new TextControl
                {
                    Text = score,
                    Font = detailFont,
                    Color = textColor,
                    Position = new Vector2(200, 0)
                });

            // Time
            panel.AddChild(
                new TextControl
                    {
                        Text = String.Format("Completed in {0}", time),
                        Font = detailFont,
                        Color = textColor,
                        Position = new Vector2(400, 0)
                    });

            return panel;
        }
    }
}
