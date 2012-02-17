namespace NathanielGame
{
    class Gun : ProjectileWeapon
    {

        private readonly Bullet _bullet;
        public Gun(GameplayScreen gameplayScreen, GameCharacter owner)
            :base(gameplayScreen, owner)
        {
            _bullet = new Bullet(ImageManager.BulletTexture);
            coolDownTime = 0.8;
            coolDownElapsed = 0;
        }

        public override void Shoot(double elapsedTime)
        {
            if(Shoot(elapsedTime, _bullet))
            {
                Audio.GunShot();
            }
        }
    }
}
