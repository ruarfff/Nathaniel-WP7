using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class HealthBar : DrawableGameAgent
    {
        private readonly GameCharacter _owner;
        private int _width;
        private const int Height = 10;
        private int _xPos;
        private int _yPos;

        public HealthBar(Texture2D texture, GameCharacter owner)
        {
            this.texture = texture;
            
            _owner = owner;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Update()
        {
            _width = _owner.Body.Width;
            _xPos = (int)(_owner.Position.X);
            _yPos = _owner.Body.Top-Height;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
                //Draw the negative space for the health bar
                spriteBatch.Draw(texture, new Rectangle(_xPos,_yPos,
                    _width, Height), new Rectangle(0, texture.Height/2, texture.Width, Height), Color.Red);


                //Draw the current health level based on the current Health
                spriteBatch.Draw(texture, new Rectangle(_xPos, _yPos,
                    (int)(_width * ((double)_owner.CurrentHP / _owner.MaxHP)), Height),
                     new Rectangle(0, texture.Height / 2, texture.Width, Height), Color.ForestGreen);

                //Draw the box around the health bar
                spriteBatch.Draw(texture, new Rectangle(_xPos, _yPos,
                    _width, Height), new Rectangle(0, 0, texture.Width, Height), Color.White);               
        }

    }
}
