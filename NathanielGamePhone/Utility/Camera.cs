using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace NathanielGame
{
    class Camera
    {
        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        private readonly int _width;
        public int Width
        {
            get { return _width; }
        }
        private readonly int _height;
        public int Height
        {
            get { return _height; }
        }
        private Vector2 _target;
        public Vector2 Target
        {
            set { _target = value; }
        }
        private readonly Rectangle _clampRect;
        public Rectangle ClampRect
        {
            get { return _clampRect; }
        }

        private static Rectangle _visibleArea;
        public static Rectangle VisibleArea
        {
            get { return _visibleArea; }
        }

        private static Vector2 _center;
        public static Vector2 Center
        {
            get
            {
                _center.X = _visibleArea.Center.X;
                _center.Y = _visibleArea.Center.Y;
                return _center;
            }
        }
        private const float Speed = 0.2f;

        /// <summary>
        /// Initialise the camera, using the game map to define the boundaries
        /// </summary>
        /// <param name="vp">Graphics viewport</param>
        /// <param name="map">Game Map</param>
        public Camera(Viewport vp, Map map)
            : this(vp, new Rectangle(0, 0, (map.Width * map.TileWidth), (map.Height * map.TileHeight)))
        { }

        /// <summary>
        /// Initialise the camera, using a custom rectangle to define the boundaries
        /// </summary>
        /// <param name="vp">Graphics viewport</param>
        /// <param name="clampRect">A rectangle defining the map boundaries, in pixels</param>
        public Camera(Viewport vp, Rectangle clampRect)
        {
            _position = new Vector2(0, 0);
            _width = vp.Width;
            _height = vp.Height;
            _center = Vector2.Zero;
            _clampRect = clampRect;

            // Set initial position and target
            _position.X = _clampRect.X;
            _position.Y = _clampRect.Y;
            _target = new Vector2(_clampRect.X, _clampRect.Y);
            _visibleArea = new Rectangle((int)_position.X, (int)_position.Y, _width, _height);
        }

        /// <summary>
        /// Update the camera
        /// </summary>
        public void Update()
        {
            // Clamp target to map/camera bounds
            _target.X = (int)MathHelper.Clamp(_target.X, _clampRect.X, _clampRect.Width - _width);
            _target.Y = (int)MathHelper.Clamp(_target.Y, _clampRect.Y, _clampRect.Height - _height);

            // Move camera toward target
            _position = Vector2.SmoothStep(_position, _target, Speed);
            _visibleArea.X = (int)_position.X;
            _visibleArea.Y = (int)_position.Y;
        }
    }
}
