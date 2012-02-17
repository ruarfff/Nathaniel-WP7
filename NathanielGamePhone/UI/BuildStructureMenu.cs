using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using NathanielGame.GameAgents.GameCharacters.Structures;
using NathanielGame.Levels;
using TiledLib;

namespace NathanielGame
{
    class BuildStructureMenu : DrawableGameComponent
    {
        private static GameplayScreen _gameplayScreen;
        private Texture2D _gradientTexture;
        private Vector2 _position;
        private Viewport _vp;
        private static BuildStructureMenuItem _selectedItem;
        private Rectangle _backgroundRectangle;
        public Rectangle BackgroundRectangle
        {
            get { return _backgroundRectangle; }
        }
        private float _transitionAlpha;
        public static float MenuHeight
        {
            get { return _menuHeight; }
        }
        private static float _menuHeight;
        public static float MenuWidth
        {
            get { return _menuWidth; }
        }

        private static float _menuWidth;
        public float MenuSlotWidth
        {
            get { return _menuWidth / 4; }
        }

        private static Rectangle _dropArea;
        private static Color _dropAreaColour;
        public static float MenuItemWidth { get; private set; }
        public static float MenuItemHeight { get; private set; }
        private static float _margin;

        private Color _color;
        private static bool _isActive;
        public static bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        private static List<BuildStructureMenuItem> _items;
        public static List<BuildStructureMenuItem> Items
        {
            get { return _items; }
        }

        public BuildStructureMenu(Game game, GameplayScreen gameplayScreen)
            : base(game)
        {
            _gameplayScreen = gameplayScreen;
            _vp = _gameplayScreen.ScreenManager.GraphicsDevice.Viewport;
        }
        public override void Initialize()
        {
            _menuWidth = _vp.Width * 0.8f;
            _menuHeight = _vp.Height * 0.3f;
            _margin = _menuWidth/24f;
            MenuItemWidth = (int)(_gameplayScreen.VP.Width * 0.06);
            MenuItemHeight = (int)(_gameplayScreen.VP.Height * 0.1);
            _position = new Vector2(_vp.X + _vp.Width * 0.1f, _vp.Y + _vp.Height * 0.6f);
            _backgroundRectangle = new Rectangle((int)_position.X, (int)_position.Y, (int)_menuWidth, (int)_menuHeight);
            _gradientTexture = ImageManager.GradientTexture;
            _transitionAlpha = 0.5f;
            _items = new List<BuildStructureMenuItem>();
            _dropArea = new Rectangle();
            BuildStructureMenuItem a = new BuildStructureMenuItem(ImageManager.GunTowerTexture, "Gun Tower", 5);
            _items.Add(a);
            BuildStructureMenuItem b = new BuildStructureMenuItem(ImageManager.LaserTowerTexture, "Laser Tower", 10);
            _items.Add(b);
            BuildStructureMenuItem c = new BuildStructureMenuItem(ImageManager.HealTowerTexture, "Heal Tower", 15);
            _items.Add(c);

            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].PositionInMenu(new Rectangle((int)(_position.X + MenuSlotWidth * i), (int)(_position.Y+_margin),
                                                       (int)MenuItemWidth, (int)MenuItemHeight));
            }
            _selectedItem = null;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if(_selectedItem != null)
            {
                _selectedItem.UpdateItem(gameTime);
                _dropArea.X = (int)_selectedItem.Position.X;
                _dropArea.Y = (int)_selectedItem.Position.Y - (_selectedItem.Body.Height*2);
                _dropArea.Width = _selectedItem.Body.Width;
                _dropArea.Height = _selectedItem.Body.Height;
                _dropAreaColour = CheckBuildArea() ? Color.LawnGreen*0.5f : Color.Red*0.5f;
            }
            if (!IsActive) return;
            foreach (var item in _items)
            {
                item.UpdateItem(gameTime);
            }
            base.Update(gameTime);
        }

        public void DrawMenu(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(_selectedItem != null)
            {
                spriteBatch.Draw(_gradientTexture, _dropArea, _dropAreaColour);
                _selectedItem.DrawItem(spriteBatch);
                
            }
            if (!_isActive) return;
            // Fade the popup alpha during transitions.
            _color = Color.White * _transitionAlpha;
            // Draw the background rectangle.
            spriteBatch.Draw(_gradientTexture, _backgroundRectangle, _color);
            
            foreach (var item in _items)
            {
                spriteBatch.DrawString(_gameplayScreen.ScreenManager.Font, item.Identifier, new Vector2(item.Body.X, _backgroundRectangle.Top), Color.White);
                spriteBatch.DrawString(_gameplayScreen.ScreenManager.Font, item.Cost.ToString(), new Vector2(item.Body.X+item.Body.Width/2, item.Body.Bottom), Color.White);
                item.DrawItem(spriteBatch);
            }
            base.Draw(gameTime);
        }

        public static void HandleBuildMenuSelection(Point screenInputTouchPosition, GestureSample gest)
        {
            //try
            //{
                if (_selectedItem == null && IsActive)
                {
                    foreach (var menuItem in
                        Items.Where(menuItem => menuItem.Body.Contains(screenInputTouchPosition)).
                            Where(
                                menuItem => menuItem.Cost <= Player.resources))
                    {
                        _selectedItem = menuItem;
                        IsActive = false;
                    }
                }
                else if (_selectedItem != null)
                {
                    _selectedItem.Center = gest.Position;
                }
          //  }
            //catch (Exception e)
            //{
              //  Debug.WriteLine("Problem at build menu selection" + e.Message);
            //}
        }

        public static void TryPlaceDefensiveStructure()
        {
            if (_selectedItem != null && !IsActive)
            {
                if (CheckBuildArea())
                {
                    Vector2 placePosition = new Vector2(_dropArea.Center.X, _dropArea.Center.Y) + GameplayScreen.Camera.Position;
                    DefensiveStructure tower;
                    switch (_selectedItem.Identifier)
                    {
                        case ("Gun Tower"):
                            tower = new GunTower(_gameplayScreen) { Center = placePosition };
                            break;
                        case ("Laser Tower"):
                            tower = new LaserTower(_gameplayScreen) { Center = placePosition };
                            break;
                        case ("Heal Tower"):
                            tower = new HealTower(_gameplayScreen) { Center = placePosition };
                            break;
                        default:
                            tower = null;
                            break;

                    }
                    if (tower != null)
                    {
                        tower.Initialize();
                        PlayerManager.PlayerCharacters.Add(tower);
                        Player.resources -= _selectedItem.Cost;
                    } 
                }
                _selectedItem.Center = _selectedItem.MenuPositionCenter;
                _selectedItem = null;
                IsActive = false;
            }
        }

        private static bool CheckBuildArea()
        {
            Vector2 placePosition = new Vector2(_dropArea.X, _dropArea.Y) + GameplayScreen.Camera.Position;
            Rectangle checkArea = new Rectangle((int)placePosition.X, (int)placePosition.Y, _dropArea.Width, _dropArea.Height);
            return !TiledMap.GetMapSquareCollision(_dropArea)
                && EnemyManager.Enemies.All(t => !checkArea.Intersects(t.Body))
                && EnemyManager.Corpses.All(t => !checkArea.Intersects(t.Body))
                && PlayerManager.PlayerCharacters.All(t => !checkArea.Intersects(t.Body))
                && !PlayerManager.Hermes.Body.Intersects(checkArea)
                && !PlayerManager.Nathaniel.Body.Intersects(checkArea);
        }
    }
}
