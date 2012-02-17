using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    abstract class BeamWeapon : Weapon, IRangedWeapon
    {
        protected BasicPrimitives beam;
        protected double burstDuration;
        protected double burstDurationElapsed;
        protected int dps;
        private float _timer;

        protected BeamWeapon(GameplayScreen gameplayScreen, GameCharacter owner) : base(gameplayScreen, owner)
        {
            burstDurationElapsed = 0;
        }

        public override void Update()
        {
            if(IsBeingUsed && owner.HasTarget && Vector2.Distance(owner.Center, owner.Target.Center) < owner.Range)
            {
                _timer += owner.Elapsed;
                if (_timer >= 1)
                {
                    if(owner.Target.CurrentHP > 0)
                    owner.Target.CurrentHP -= dps;
                    _timer = 0;
                }
            }
            else
            {
                burstDurationElapsed = 0;
                beam.ClearVectors();
                IsBeingUsed = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(IsBeingUsed && owner.IsAlive)
            beam.RenderPolygonPrimitive(spriteBatch);
        }

        public override void Use(double elapsedTime)
        {
            Shoot(elapsedTime);
        }

        public abstract void Shoot(double elapsedTime);
    }
}
