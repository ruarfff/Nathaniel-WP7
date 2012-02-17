using Microsoft.Xna.Framework;

namespace NathanielGame
{
    class Laser : BeamWeapon
    {
        public Color LaserColour { get; set; }
        public int LaserThickness { get; set; }
        public Laser(GameplayScreen gameplayScreen, GameCharacter owner) : base(gameplayScreen, owner)
        {
            beam = new BasicPrimitives(gamePlayScreen.ScreenManager.GraphicsDevice);
            //Default beam colour can be changed with public property
            LaserColour = new Color(255, 255, 255, 180);
            LaserThickness = 2;
            coolDownTime = 3.5;
            coolDownElapsed = 0;
            burstDuration = 1.5;
            dps = 20;
        }

        public override void Shoot(double elapsedTime)
        {
            coolDownElapsed += elapsedTime;
            if (coolDownElapsed < coolDownTime && !IsBeingUsed) return;
            coolDownElapsed = 0;
            
            if (burstDurationElapsed >= burstDuration)
            {
                IsBeingUsed = false;
                return;
            }
            burstDurationElapsed += elapsedTime;
            if (burstDurationElapsed > 0.5f && burstDurationElapsed < 0.7f)
            {
                if (owner.Name != null)
                {
                    if (owner.Name.StartsWith("He"))
                    {
                        Audio.RayGun();
                        dps = 25;

                    }
                    else if (owner.Name.StartsWith("Sp"))
                    {
                        Audio.LaserCannon();
                        dps = 30;
                    }
                    else
                    {
                        Audio.LaserBlast();
                    }
                }
                else
                {
                    Audio.LaserBlast();
                }
            }
            beam.ClearVectors();
            beam.AddVector(owner.Center); // Vector2
            beam.AddVector(owner.Target.Center); // Vector2
            beam.Colour = LaserColour;
            beam.Thickness = LaserThickness;
            IsBeingUsed = true;
        }
    }
}
