using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NathanielGame
{
    class Animation
    {
        #region Fields
        private readonly Texture2D _animatedCharacter;
        private readonly int _frameCount;
        private Point _frameSize;
        private readonly int _animationRow;
        private readonly int _fps;
        private bool _drawWasAlreadyCalledOnce;
        private int _frameIndex;
        private bool _isHorizontal;
        public bool IsActive { get; private set; }
        private Rectangle _currentFrame;
        #endregion

        #region Initialization
        /// <summary>
        /// Creates a new instance of the animation class
        /// </summary>
        /// <param name="frameSheet">Texture which is a sheet containing 
        /// the animation frames.</param>
        /// <param name="frameSize">The size of a single frame.</param>
        /// <param name="frameCount">The number of frames to index.</param>
        /// <param name="framesPerSecond">The number of frames to be displayed in one second.</param>
        /// <param name="animationRow">To claculate the y position of the animation.</param>
        /// <param name="isHorizontal">To decide if the animation reads up and down or left to right on the sprite sheet</param>
        public Animation(Texture2D frameSheet, Point frameSize, int frameCount, int framesPerSecond, int animationRow, bool isHorizontal)
        {
            _animatedCharacter = frameSheet;
            _frameSize = frameSize;
            _frameCount = frameCount;
            _fps = framesPerSecond;
            _animationRow = animationRow;
            _isHorizontal = isHorizontal;
            _currentFrame = new Rectangle(0, _animationRow*_frameSize.Y, _frameSize.X, _frameSize.Y);
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates the animation's progress.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        /// <param name="isInMotion">Whether or not the animation element itself is
        /// currently in motion.</param>
        public void Update(GameTime gameTime, bool isInMotion)
        {
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            _frameIndex = 0; // Reset the animation
            if (!isInMotion) return;
            // Do not advance frames before the first draw operation
            if (_drawWasAlreadyCalledOnce)
            {
                _frameIndex += (int)(time * _fps) % _frameCount;
            }
            _currentFrame.X = _frameSize.X*_frameIndex;
        }
        public void Draw(SpriteBatch spriteBatch, Rectangle drawArea)
        {
            _drawWasAlreadyCalledOnce = true;
            spriteBatch.Draw(_animatedCharacter, drawArea, _currentFrame, Color.White);
          
        }
        /// <summary>
        /// Causes the animation to start playing from a specified frame index.
        /// </summary>
        /// <param name="frameIndex">Frame index to play the animation from.</param>
        public void PlayFromFrameIndex(int frameIndex)
        {
            _frameIndex = frameIndex;
            IsActive = true;
            _drawWasAlreadyCalledOnce = false;
        }

        #endregion
    }
}
