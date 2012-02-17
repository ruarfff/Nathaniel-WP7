using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NathanielGame
{
    class Bullet : Projectile
    {
        public Bullet(Texture2D texture)
            : base(texture)
        {
        }

        public override void Initialize()
        {
            maxDistance = 600;
            speed = 15.0f;
            damage = 25;
            drawScale = 1.0f;
            drawColour = Color.White;
            base.Initialize();
            body.Width = (int)(texture.Width*drawScale);
            body.Height = (int)(texture.Height*drawScale);
        }
    }
}
