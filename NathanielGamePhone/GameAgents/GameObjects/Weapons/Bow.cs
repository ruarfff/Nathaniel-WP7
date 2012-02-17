namespace NathanielGame
{
    class Bow : ProjectileWeapon
    {
        private readonly Arrow _arrow;
        public Bow(GameplayScreen gameplayScreen, GameCharacter owner)
            :base(gameplayScreen, owner)
        {
            _arrow = new Arrow(ImageManager.ArrowTexture);
            coolDownTime = 1.5;
            coolDownElapsed = 0;
        }
        
        public override void Shoot(double elapsedTime)
        {
            if(Shoot(elapsedTime, _arrow))
            {
                Audio.ArrowShot();
            }
        }
    }
}
