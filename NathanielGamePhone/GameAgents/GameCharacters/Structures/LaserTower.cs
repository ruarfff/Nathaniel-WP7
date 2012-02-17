using Microsoft.Xna.Framework;

namespace NathanielGame.GameAgents.GameCharacters.Structures
{
    class LaserTower : DefensiveStructure
    {
        public LaserTower(GameplayScreen gamePlayScreen) : base(gamePlayScreen)
        {
        }

        public override void Initialize()
        {
            //PrimaryWeapon
            PrimaryWeapon = new Laser(gamePlayScreen, this){LaserColour = Color.Red, LaserThickness = 3};
            isArmed = true;
            range = 350;
            visibleRange = 400f;


            //Hit points
            maxHP = 600;
            startingHP = maxHP;
            currentHP = startingHP;

            //Image
            texture = ImageManager.LaserTowerTexture;
            width = (int)(gamePlayScreen.VP.Width * 0.06);
            height = (int)(gamePlayScreen.VP.Height * 0.1);
            base.Initialize();
        }
    }
}
