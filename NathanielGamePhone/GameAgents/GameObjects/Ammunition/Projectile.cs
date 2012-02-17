using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    abstract class Projectile : DrawableGameAgent
    {
        protected int maxDistance;
        protected float drawScale;
        protected Vector2 startPosition;
        protected Vector2 direction;
        protected Vector2 destination;
        protected float faceDirection;
        protected Color drawColour;
        protected float speed;
        protected int damage;
        public int Damage
        {
            get { return damage; }
        }
        public bool IsVisible
        {
            get { return (Vector2.Distance(startPosition, Center) < maxDistance && Center != destination && !HasCollision); }
        }

        public override Rectangle Body
        {
            get
            {
                body.X = (int)(Position.X - (texture.Width * drawScale) / 2);
                body.Y = (int) (Position.Y - (texture.Height*drawScale)/2);
                return body;
            }
        }
        protected Projectile(Texture2D texture)
        {
            this.texture = texture;
        }

        public void Update()
        {
            if (IsVisible)
            {
                Center += direction*speed;
            }
            else
            {
                Center = Vector2.Zero;
                destination = Vector2.Zero;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                spriteBatch.Draw(texture,Position,null,drawColour,faceDirection,
                    Vector2.Zero,drawScale,SpriteEffects.None, 0.0f);
            }
        }

        public void Fire(Vector2 fireFrom, Vector2 target)
        {

            HasCollision = false;
            Center = fireFrom;
            startPosition = fireFrom;
            destination = target;
            direction = destination - Center;
            direction.Normalize();
            faceDirection = ((float)System.Math.Atan2(-direction.Y, -direction.X) + MathHelper.TwoPi) % MathHelper.TwoPi;
        }
    }
}
