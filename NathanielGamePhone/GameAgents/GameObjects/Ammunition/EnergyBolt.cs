using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class EnergyBolt : Projectile
    {
        public EnergyBolt(Texture2D texture)
            : base(texture)
        {
            
        }
        public override void Initialize()
        {
            maxDistance = 500;
            speed = 20.0f;
            damage = 5;
            drawScale = 1.0f;
            drawColour = Color.White;
            base.Initialize();
            body.Width = (int)(texture.Width * drawScale);
            body.Height = (int)(texture.Height * drawScale);
        }
    }
}
