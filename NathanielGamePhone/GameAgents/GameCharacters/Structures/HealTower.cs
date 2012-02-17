using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace NathanielGame
{
    class HealTower : DefensiveStructure
    {
        //Set a delay between heals
        private float _healDelay;
        private float _timer;
        public HealTower(GameplayScreen gamePlayScreen) : base(gamePlayScreen)
        {
        }

        public override void Initialize()
        {
            IsNonCombatant = true;
            //Area of effect
            range = 400;
            _healDelay = 1f;
            _timer = 0f;
            //Hit points
            maxHP = 600;
            startingHP = maxHP;
            currentHP = startingHP;

            //Image
            texture = ImageManager.HealTowerTexture;
            width = (int)(gamePlayScreen.VP.Width * 0.06);
            height = (int)(gamePlayScreen.VP.Height * 0.1);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _timer += elapsed;
            if (_timer >= _healDelay)
            {
                if (Vector2.Distance(PlayerManager.Nathaniel.Center, Center) < range &&
                    (PlayerManager.Nathaniel.CurrentHP < (PlayerManager.Nathaniel.MaxHP - 10)))
                {
                    PlayerManager.Nathaniel.CurrentHP += 5;
                    _timer = 0;
                }
                else if (Vector2.Distance(PlayerManager.Hermes.Center, Center) < range &&
                         (PlayerManager.Hermes.CurrentHP < (PlayerManager.Hermes.MaxHP - 10)))
                {
                    PlayerManager.Hermes.CurrentHP += 5;
                    _timer = 0;
                }
            }
            base.Update(gameTime);
        }

        protected override void UpdateState()
        {
            
            base.UpdateState();
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
