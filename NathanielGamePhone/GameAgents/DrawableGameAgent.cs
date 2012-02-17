using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace NathanielGame
{
    /// <summary>
    /// Abstract class to provide common functionality to all drawable game agents
    /// i.e. collision variable, animation, world position etc.
    /// </summary>
    abstract class DrawableGameAgent
    {
        #region status variables
       
        protected string name;
        /// <summary>
        /// Optional to uniquely identify objects by the name allocated to them by tiled qt
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// Used to determine if the object should be disposed of
        /// </summary>
        public bool IsActive { get; protected set; }
        #endregion
        #region animation
        /// <summary>
        ///Does this object animate. If true the texture will be seperated in to a sprite sheet
        /// </summary>
        protected bool animates;
        protected int spriteSheetCols;
        protected int spriteSheetRows;
        protected int frameWidth;
        protected int frameHeight;
        protected int animationFramesPerSecond;
        protected Rectangle currentFrame;
        protected Rectangle[,] frames;
        #endregion
        #region drawing variables
        protected Vector2 position;
        /// <summary>
        /// The x0 y0 top left position of this object in the 
        /// game world. For collision and movement the Center property should be used
        /// </summary>
        public Vector2 Position
        {
            get
            {
                position.X = Center.X - width/2f;
                position.Y = Center.Y - height/2f;
                return position; 
            }
        }
        protected Rectangle body;
        /// <summary>
        /// This is the space the object takes up in the world
        /// </summary>
        public virtual Rectangle Body
        {
            get
            {
                body.X = (int)Position.X;
                body.Y = (int)Position.Y;
                return body;
            }
        }
        protected int width;
        protected int height;
        /// <summary>
        /// The main image used for rendering this object
        /// If this is a sprites sheet it will be devided upo based on the 
        /// number of cols and rows specified
        /// </summary>
        protected Texture2D texture;
        #endregion
        #region Collision Detection Methods
        public Vector2 Center { get; set; }
        public float CollisionRadius { get { return width*0.4f; } }
        protected bool hasCollision;
        public bool HasCollision
        {
            get { return hasCollision; }
            set { hasCollision = value; }
        }
        /// <summary>
        /// The least accurate collision method but the least costly.
        /// This checks if the rectangular area of this object intersects another 
        /// rectangle belonging to a DrawableGameAgent
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsBoxColliding(DrawableGameAgent other)
        {
            return Body.Intersects(other.Body);
        }
        /// <summary>
        /// Slightly more accurate than box collision. 
        /// Checks if there is a collision within a radius around this objects
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsCircleColliding(DrawableGameAgent other)
        {
            return Vector2.Distance(Center, other.Center) < (CollisionRadius);
            //(CollisionRadius + other.CollisionRadius);
        }
        #endregion   
        /// <summary>
        /// Used to get access to and relate this to the screen it will be drawn on
        /// </summary>
        protected GameplayScreen gamePlayScreen;
        
        public virtual void Initialize()
        {
            IsActive = true;
            position = new Vector2();
            body = new Rectangle {X=(int)Position.X, Y=(int)Position.Y, Width = width, Height = height};
            if (!animates) return;
            //Cut up sprite sheet in to rectangles for animation
            if (spriteSheetCols > 0 && spriteSheetRows > 0)
            {
                frameWidth = texture.Width/spriteSheetCols;
                frameHeight = texture.Height/spriteSheetRows;
                frames = new Rectangle[spriteSheetRows,spriteSheetCols];
                for (int i = 0; i < spriteSheetRows; i++)
                {
                    for (int j = 0; j < spriteSheetCols; j++)
                    {
                        frames[i, j] = new Rectangle(j*frameWidth, i*frameHeight, frameWidth, frameHeight);
                    }
                }
            }
        }
    
        /// <summary>
        /// Using this draw method to reduce SpriteBatch.Begin() & End() calls
        /// If a game agent does not animate just draw the texture
        /// Can be overridden where neccessary
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(!animates)
            spriteBatch.Draw(texture, Body, Color.White);
        }
    }
}
