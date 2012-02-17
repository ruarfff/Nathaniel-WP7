using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class Corpse : DrawableGameAgent
    {
        private readonly int _resources;
        public int Resources
        {
            get { return _resources; }
        }

        public bool Removed { get; set; }
      
        private double _timeSoFar;
        private double _timeToLive;
        private bool _isCollected;  
        public bool IsCollected
        {
            get { return _isCollected; }
            set { _isCollected = value; }
        }

        public Corpse(Vector2 position, Texture2D texture, int resources)
        {
            this.texture = texture;
            Center = position;
            _resources = resources;
        }

        public override void Initialize()
        {
            Removed = false;
            _timeSoFar = 0.0;
            _timeToLive = 10.0;
            width = 70;
            height = 60;
            IsCollected = false;
            base.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            if(!_isCollected)
            {
                _timeSoFar += gameTime.ElapsedGameTime.TotalSeconds;
                
                if (_timeSoFar > _timeToLive && !_isCollected)
                {
                    Remove();
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public void Remove()
        {
            EnemyManager.Corpses.Remove(this);
            Removed = true;
            texture = null;
        }
    }
}
