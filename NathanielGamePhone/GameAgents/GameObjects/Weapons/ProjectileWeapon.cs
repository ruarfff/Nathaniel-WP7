using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    abstract class ProjectileWeapon : Weapon, IRangedWeapon
    {
        protected List<Projectile> ammo;
        public List<Projectile> Ammo
        {
            get { return ammo; }
        }

        protected ProjectileWeapon(GameplayScreen gameplayScreen, GameCharacter owner)
            : base(gameplayScreen, owner)
        {
            ammo = new List<Projectile>();
        }
        public override void Update()
        {
            foreach (var projectile in ammo)
            {
                projectile.Update();
                if (!owner.HasTarget) continue;
                if (!owner.Target.Body.Contains(new Point((int)projectile.Center.X,(int)projectile.Center.Y)) || !projectile.IsVisible) continue;
                if (owner.Target.CurrentHP > 0)
                    owner.Target.CurrentHP -= projectile.Damage;
                projectile.HasCollision = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var projectile in ammo)
            {
                projectile.Draw(spriteBatch);
            }
        }

        public override void Use(double elapsedTime)
        {
            Shoot(elapsedTime);
        }

        public abstract void Shoot(double elapsedTime);

        public bool Shoot(double elapsedTime, Projectile ammunitionType)
        {
            coolDownElapsed += elapsedTime;
            if (coolDownElapsed <= coolDownTime)
            {
                IsBeingUsed = false;
                return false;
            }
            coolDownElapsed = 0;
            IsBeingUsed = true;
            foreach (Projectile projectile in ammo.Where(projectile => !projectile.IsVisible))
            {
                projectile.Fire(owner.Center, owner.Target.Center);
                return true;
            }

            ammunitionType.Initialize();
            ammunitionType.Fire(owner.Center, owner.Target.Center);
            ammo.Add(ammunitionType);
            return true;

        }

    }
}
