using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    abstract class Weapon
    {
        protected GameCharacter owner;
        protected GameplayScreen gamePlayScreen;
        protected double coolDownTime;
        protected double coolDownElapsed;
        public bool IsBeingUsed { get; protected set; }
        
        protected Weapon(GameplayScreen gameplayScreen, GameCharacter owner)
        {
            gamePlayScreen = gameplayScreen;
            this.owner = owner;
        }

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch);

        public abstract void Use(double elapsedTime);
    }
}
