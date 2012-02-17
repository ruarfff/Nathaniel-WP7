using Microsoft.Xna.Framework;

namespace NathanielGame
{
    abstract class DefensiveStructure : GameCharacter
    {
        protected DefensiveStructure(GameplayScreen gamePlayScreen) : base(gamePlayScreen)
        {
        }

        public override void Initialize()
        {
            destination = Center;
            isInMotion = false;
            IsCurrentlyMoveable = false;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if(!IsAlive)
                Remove();
            base.Update(gameTime);
        }

        public void Remove()
        {
            AnimationManager.AddExplosion(Center, width, height);
            Audio.ExplosionSound();
            Dispose(true);
        }
    }
}
