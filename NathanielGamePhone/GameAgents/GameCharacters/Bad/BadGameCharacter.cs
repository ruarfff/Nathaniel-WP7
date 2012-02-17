using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    abstract class BadGameCharacter : GameCharacter
    {
        protected bool isSelected;
        public virtual bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        protected Rectangle selectedTextureBody;
        protected Texture2D selectedTexture;
        protected int killScore;
        protected BadGameCharacter(GameplayScreen gamePlayScreen) : base(gamePlayScreen)
        {
        }

        public override void Initialize()
        {
            selectedTexture = ImageManager.SelectedTexture;
            selectedTextureBody = new Rectangle((int)Position.X, (int)Position.Y, width, height/5);
            isSelected = false;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (IsAlive)
            {
                selectedTextureBody.X = (int)Position.X;
                selectedTextureBody.Y = Body.Bottom+10;
                base.Update(gameTime);
            }
            else
            {
                Die();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive){
                if (isSelected)
                    spriteBatch.Draw(selectedTexture, selectedTextureBody, Color.White * 0.5f);
                base.Draw(spriteBatch);
            }
        }

        protected override void Die()
        {
            Player.score += killScore;
            base.Die();
        }
    }
}
