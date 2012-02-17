using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class BuildStructureMenuItem : DrawableGameAgent
    {
        private readonly int _cost;
        public int Cost { get { return _cost; } }
        public bool IsDragged { get; set; }
        public string Identifier { get; private set; }
        public Vector2 MenuPosition { get; private set; }
        public Vector2 MenuPositionCenter { get; private set; }

        private Color DisplayStrength
        {
            get { return Player.resources >= _cost ? Color.White : Color.White*0.4f; }
        }

        public BuildStructureMenuItem(Texture2D displayTexture, string identifier, int cost) 
        {
            Identifier = identifier;
            texture = displayTexture;
            _cost = cost;
        }

        public void UpdateItem(GameTime gameTime)
        {
        }
        public void DrawItem(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Body,DisplayStrength);
        }

        public void PositionInMenu(Rectangle menuSlot)
        {
            MenuPositionCenter = new Vector2(menuSlot.X + menuSlot.Width / 2, menuSlot.Y + menuSlot.Height / 2);
            Center = MenuPositionCenter;
            body = menuSlot;
            MenuPosition = new Vector2(Position.X, Position.Y);
            IsDragged = false;
        }
    }
}
