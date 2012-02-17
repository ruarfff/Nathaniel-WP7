using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class Spawner : BadGameCharacter
    {
        private float _lastSpawnCounter;
        private float _minSpawnTime;
        private float _waitTime;
        private int _waveCount;
        private int _maxWave;
        private bool _waiting;

        public Spawner(GameplayScreen gamePlayScreen) : base(gamePlayScreen)
        {
        }

        public override void Initialize()
        {
            //Animation and images
            texture = ImageManager.SpawnerTexture;
            deadTexture = ImageManager.CorpseTexture;

            //PrimaryWeapon
            PrimaryWeapon = new Laser(gamePlayScreen, this){LaserColour = Color.SpringGreen, LaserThickness = 6};
            range = 350.0f;
            visibleRange = 400.0f;
            isArmed = true;

            //Movement and status
            destination = Center;
            isCurrentlyMoveable = false;
            isInMotion = false;
            MaxSpeed = 0f;
            Speed = MaxSpeed;
            _lastSpawnCounter = 0f;
            _minSpawnTime = 30.0f;
            _waitTime = 30;
            _waveCount = 0;
            _maxWave = 2;

            //Hit points
            maxHP = 1500;
            startingHP = maxHP;
            currentHP = maxHP;
            killScore = 100;
            
            //Initialize the draw area as a proportion of the screen size
            width = (int)(gamePlayScreen.VP.Width * 0.3f);
            height = (int)(gamePlayScreen.VP.Height * 0.35f);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {   
            base.Update(gameTime);
        }

        protected override void UpdateState()
        {
            _lastSpawnCounter += elapsed;
            if (!_waiting)
            {
                if (_lastSpawnCounter > _minSpawnTime)
                {
                    EnemyManager.AddEnemy(gamePlayScreen, "Grunt",
                                          (new Vector2(Center.X + width, Center.Y + height)));
                    _lastSpawnCounter = 0;
                    _waveCount++;
                    if (_waveCount >= _maxWave)
                    {
                        _waiting = true;
                    }
                }
            }
            else
            {
                _waitTime -= elapsed;
                if (_waitTime <= 0)
                {
                    _waiting = false;
                    _waitTime = 120;
                }
            }
            base.UpdateState();
        }

        protected override void Die()
        {
            AnimationManager.AddExplosion(Center, width, height);
            Audio.ExplosionSound();
            base.Die();
        }
    }
}
