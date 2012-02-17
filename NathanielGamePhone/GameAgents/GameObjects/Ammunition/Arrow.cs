using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NathanielGame
{
    class Arrow: Projectile
    {
        public Arrow(Texture2D texture)
            : base(texture)
        {
        }

        public override void Initialize()
        {
            maxDistance = 300;
            speed = 8.0f;
            damage = 25;
            drawScale = 1.5f;
            drawColour = Color.White;
            base.Initialize();
            body.Width = (int)(texture.Width*drawScale);
            body.Height = (int)(texture.Height*drawScale);
        }
    }
}
