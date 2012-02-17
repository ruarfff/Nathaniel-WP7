namespace NathanielGame
{
    class Blaster : ProjectileWeapon
    {
        private readonly Bullet _bullet;
        public Blaster(GameplayScreen gameplayScreen, GameCharacter owner)
            :base(gameplayScreen, owner)
        {
            _bullet = new Bullet(ImageManager.RedBulletTexture);
            coolDownTime = 0.8;
            coolDownElapsed = 0;
        }

        public override void Shoot(double elapsedTime)
        {
            if(Shoot(elapsedTime, _bullet))
            {
                Audio.LaserSound();
            }
        }
    }
    
}
