using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    static class AnimationManager
    {
        private static List<StripAnimation> _explosions;
        private static Texture2D _nathanielSpriteSheet;
        public static Texture2D NathanielSpriteSheet
        {
            get { return _nathanielSpriteSheet; }
        }
        private static Dictionary<string, Texture2D> _sheets;
        public static Dictionary<string, Animation> Animations { get; private set; }
        

        public static void Initialize()
        {
            // Initialize the explosion list
            _explosions = new List<StripAnimation>();
        }

        public static void LoadContent(ContentManager content)
        {

        }
        
        public static void Update(GameTime gameTime)
        {
            UpdateExplosions(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            // Draw the explosions
            for (int i = 0; i < _explosions.Count; i++)
            {
                _explosions[i].Draw(spriteBatch);
            }
        }


        #region Explosion
        public static void AddExplosion(Vector2 position, int width, int height)
        {
            StripAnimation explosion = new StripAnimation();
            explosion.Initialize(ImageManager.ExplosionSheetOne, position, 134, 134, 12, 45, Color.White, 1f, false);
            _explosions.Add(explosion);
        }

        private static void UpdateExplosions(GameTime gameTime)
        {
            for (int i = _explosions.Count - 1; i >= 0; i--)
            {
                _explosions[i].Update(gameTime);
                if (_explosions[i].Active == false)
                {
                    _explosions.RemoveAt(i);
                }
            }
        }
        #endregion

        
        public static void LoadAnimiationFromXML(ContentManager content)
        {
            _sheets = new Dictionary<string, Texture2D>();
            Animations = new Dictionary<string, Animation>();

           _nathanielSpriteSheet = content.Load<Texture2D>("Textures/Characters/SpriteSheets/nathanielspritesheet");
            _sheets.Add("nathanielspritesheet", _nathanielSpriteSheet);
           
            
            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load("Content/Textures/Characters/AnimationDefinitions.xml");
            System.Xml.Linq.XName name = System.Xml.Linq.XName.Get("Definition");
            if (doc.Document != null)
            {
                var definitions = doc.Document.Descendants(name);

                // Loop over all definitions in the XML
                foreach (var animationDefinition in
                    definitions.Where(animationDefinition => animationDefinition != null))
                {
                        string animatonAlias = animationDefinition.Attribute("Alias").Value;
                        string sheet = animationDefinition.Attribute("SheetName").Value;
                        // Get the frame size (width & height)
                        var frameSize = new Point
                                            {
                                                X = int.Parse(animationDefinition.Attribute("FrameWidth").Value),
                                                Y = int.Parse(animationDefinition.Attribute("FrameHeight").Value)
                                            };
                        int fps = int.Parse(animationDefinition.Attribute("Speed").Value);
                        //bool isHorizontal = Boolean.Parse(animationDefinition.Attribute("IsHorizontal").Value);
                        // Get the frames sheet dimensions
                        bool isHorizontal = false;
                        int animationRow = int.Parse(animationDefinition.Attribute("SheetRow").Value);
                        int frameCount = int.Parse(animationDefinition.Attribute("SheetColumns").Value);

                        var animation = new Animation(_sheets[sheet], frameSize, frameCount, fps, animationRow, isHorizontal);
    
                        Animations.Add(animatonAlias, animation);
                    }
          
                }
        }
    }
}
