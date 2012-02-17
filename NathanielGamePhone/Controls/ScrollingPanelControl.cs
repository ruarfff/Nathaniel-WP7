using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace NathanielGame
{
    class ScrollingPanelControl : PanelControl
    {
        private ScrollTracker scrollTracker = new ScrollTracker();

        public override void Update(GameTime gametime)
        {
            Vector2 size = ComputeSize();
            scrollTracker.CanvasRect.Width = (int)size.X;
            scrollTracker.CanvasRect.Height = (int)size.Y;
            scrollTracker.Update(gametime);

            base.Update(gametime);
        }

        public override void HandleInput(InputState input)
        {
            scrollTracker.HandleInput(input);
            base.HandleInput(input);
        }

        public override void Draw(DrawContext context)
        {
            // To render the scrolled panel, we just adjust our offset before rendering our child controls as
            // a normal PanelControl
            context.DrawOffset.Y = -scrollTracker.ViewRect.Y;
            base.Draw(context);
        }
    }
}
