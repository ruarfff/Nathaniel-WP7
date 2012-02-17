using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    static class ImageManager
    {
        public static SpriteFont GameFont { get; set; }
        public static Texture2D HermesIdleSpriteSheet { get; set; }
        public static Texture2D HermesMovingSpriteheet { get; set; }
        public static Texture2D SpawnerTexture { get; set; }
        public static Texture2D GreenEnemyTexture { get; set; }
        public static Texture2D GruntTexture { get; set; }
        public static Texture2D Boss1Texture { get; set; }
        public static Texture2D GameOverImage { get; set; }
        public static Texture2D SurvivalGameOverImage { get; set; }
        public static Texture2D WinScreenImage { get; set; }
        public static Texture2D RedBulletTexture { get; set; }
        public static Texture2D ExplosionSheetOne { get; set; }
        public static Texture2D ExplosionSheetTwo { get; set; }

        private static Texture2D _clockTexture;
        public static Texture2D ClockTexture
        {
            get { return _clockTexture; }
        }
        private static Texture2D _nathanielIcon;
        public static Texture2D NathanielIcon
        {
            get { return _nathanielIcon; }
        }
        private static Texture2D _hermesIcon;
        public static Texture2D HermesIcon
        {
            get { return _hermesIcon; }
        }
        private static Texture2D _buildModeIcon;
        public static Texture2D BuildModeIcon
        {
            get { return _buildModeIcon; }
        }
        private static Texture2D _moveModeIcon;
        public static Texture2D MoveModeIcon
        {
            get { return _moveModeIcon; }
        }
        private static Texture2D _heartIcon;
        public static Texture2D HeartIcon
        {
            get { return _heartIcon; }
        }
        private static Texture2D _menuButtonIcon;
        public static Texture2D MenuButtonIcon
        {
            get { return _menuButtonIcon; }
        }

        private static Texture2D _healthBarTexture;
        public static Texture2D HealthBarTexture
        {
            get { return _healthBarTexture; }
        }
        private static Texture2D _selectedTexture;
        public static Texture2D SelectedTexture
        {
            get { return _selectedTexture; }
        }
        private static Texture2D _bulletTexture;
        public static Texture2D BulletTexture
        {
            get { return _bulletTexture; }
        }
        private static Texture2D _corpseTexture;
        public static Texture2D CorpseTexture
        {
            get { return _corpseTexture; }
        }

        private static Texture2D _bloodTexture;
        public static Texture2D BloodTexture
        {
            get { return _bloodTexture; }
        }
        private static Texture2D _shadowTexture;
        public static Texture2D ShadowTexture
        {
            get { return _shadowTexture; }
        }

        private static SpriteFont _hudFont;
        public static SpriteFont HudFont
        {
            get { return _hudFont; }
        }

        private static Texture2D _gunTowerTexture;
        public static Texture2D GunTowerTexture
        {
            get { return _gunTowerTexture; }
        }

        private static Texture2D _laserTowerTexture;
        public static Texture2D LaserTowerTexture
        {
            get { return _laserTowerTexture; }
        }

        private static Texture2D _healTowerTexture;
        public static Texture2D HealTowerTexture
        {
            get { return _healTowerTexture; }
        }
        private static Texture2D _gradientTexture;
        public static Texture2D GradientTexture
        {
            get { return _gradientTexture; }
        }

        private static Texture2D _arrowTexture;
        public static Texture2D ArrowTexture
        {
            get { return _arrowTexture; }
        }

        private static Texture2D _nathanielTexture;
        public static Texture2D NathanielTexture
        {
            get { return _nathanielTexture; }
        }

        private static Texture2D _reumeButtonIcon;
        public static Texture2D ResumeButtonIcon
        {
            get { return _reumeButtonIcon; }
        }
        private static Texture2D _saveButtonIcon;
        public static Texture2D SaveButtonIcon
        {
            get { return _saveButtonIcon; }
        }
        public static void LoadContent(ContentManager content)
        {
            GameFont = content.Load<SpriteFont>("Fonts/gamefont");
            _nathanielTexture = content.Load<Texture2D>("Textures/Characters/SpriteSheets/nathanielspritesheet");
            _healthBarTexture = content.Load<Texture2D>("Textures/Objects/healthbar");
            _selectedTexture = content.Load<Texture2D>("Textures/Objects/selected");
            _corpseTexture = content.Load<Texture2D>("Textures/Objects/corpse");
            _bloodTexture = content.Load<Texture2D>("Textures/Objects/blood");
            _bulletTexture = content.Load<Texture2D>("Textures/Objects/bullet");
            _shadowTexture = content.Load<Texture2D>("Textures/Characters/shadow");
            _gunTowerTexture = content.Load<Texture2D>("Textures/Structures/guntower");
            _laserTowerTexture = content.Load<Texture2D>("Textures/Structures/lasertower");
            _healTowerTexture = content.Load<Texture2D>("Textures/Structures/healtower");
            _gradientTexture = content.Load<Texture2D>("Textures/Backgrounds/gradient");
            _arrowTexture = content.Load<Texture2D>("Textures/Objects/arrow");
            _saveButtonIcon = content.Load<Texture2D>("Textures/Icons/savebuttonicon");
            _reumeButtonIcon = content.Load<Texture2D>("Textures/Icons/resumebuttonicon");
            HermesIdleSpriteSheet = content.Load<Texture2D>("Textures/Characters/SpriteSheets/hermesidlespritesheet");
            HermesMovingSpriteheet = content.Load<Texture2D>("Textures/Characters/SpriteSheets/hermesmovingspritesheet");
            SpawnerTexture = content.Load<Texture2D>("Textures/Characters/Idle/Spawner");
            GreenEnemyTexture = content.Load<Texture2D>("Textures/Characters/SpriteSheets/greenenemyspritesheet");
            GruntTexture = content.Load<Texture2D>("Textures/Characters/SpriteSheets/gruntspritesheet");
            Boss1Texture = content.Load<Texture2D>("Textures/Characters/SpriteSheets/boss1spritesheet");
            GameOverImage = content.Load<Texture2D>("Textures/Icons/gameoverimage");
            SurvivalGameOverImage = content.Load<Texture2D>("Textures/Icons/survivalgameoverimage");
            ExplosionSheetTwo = content.Load<Texture2D>("Textures/Characters/SpriteSheets/explode");
            WinScreenImage = content.Load<Texture2D>("Textures/Icons/winscreenimage");
            RedBulletTexture = content.Load<Texture2D>("Textures/Objects/redbullet");
            ExplosionSheetOne = content.Load<Texture2D>("Textures/Characters/explosion");
            _clockTexture = content.Load<Texture2D>("Textures/Objects/Clock");
            _hermesIcon = content.Load<Texture2D>("Textures/Icons/HermesIcon");
            _nathanielIcon = content.Load<Texture2D>("Textures/Icons/NathanielIcon");
            _moveModeIcon = content.Load<Texture2D>("Textures/Icons/FollowModeIcon");
            _buildModeIcon = content.Load<Texture2D>("Textures/Icons/BuildModeIcon");
            _heartIcon = content.Load<Texture2D>("Textures/Icons/hearticon");
            _hudFont = content.Load<SpriteFont>("Fonts/hudfont");
            _menuButtonIcon = content.Load<Texture2D>("Textures/Icons/menubuttonicon");
        }
    }
}
