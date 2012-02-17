using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace NathanielGame
{
    class GunTower : DefensiveStructure
    {  
        public GunTower(GameplayScreen gamePlayScreen) : base(gamePlayScreen)
        {
        }

        public override void Initialize()
        {
            //PrimaryWeapon
            PrimaryWeapon = new Gun(gamePlayScreen,this);
            isArmed = true;
            range = 500;
            visibleRange = 700f;
            

            //Hit points
            maxHP = 600;
            startingHP = maxHP;
            currentHP = maxHP;

            //Image
            texture = ImageManager.GunTowerTexture;
            width = (int)(gamePlayScreen.VP.Width*0.06);
            height = (int) (gamePlayScreen.VP.Height*0.1);
            base.Initialize();
        }
    }
}
