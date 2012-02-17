using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using NathanielGame.Levels;
using TiledLib;

namespace NathanielGame
{
    /// <summary>
    /// This class is used for path finding. It simply holds information about the current 
    /// tiled map in use at the gameplay screen
    /// </summary>
    abstract class TiledMap
    {
        #region Declarations
        //Map that has been loaded in the gameplay screen
        private static Map _map;
        //A 2D grid of map squares
        private static int[,] _mapSquares;

        private const int ContainsCharacter = 2;
        private const int Collision = 1;
        private const int NonCollision = 0;

        private static int _tileWidth;
        private static int _tileHeight;

        #endregion

        #region Initialization
        static public void Initialize()
        {
            _map = Level.CurrentMap;
            //map width and height is measured in tiles. 
            _mapSquares = new int[_map.Width, _map.Height];
            //Tile width is measured in pixels
            _tileWidth = _map.TileWidth;
            _tileHeight = _map.TileHeight;

            var colLayer = _map.GetLayer("Collision");
            var tileLayer = colLayer as TileLayer;
            //Iterate through the map. If collision layers tile at index is null then 
            //it is marked as no collision in our grid
            for (int x = 0; x < _map.Width; x++)
                for (int y = 0; y < _map.Height; y++)
                {
                    if (tileLayer != null)
                        if(tileLayer.Tiles[x,y] == null)
                        {
                            _mapSquares[x, y] = NonCollision;
                        }
                        else
                        {
                            //Collision is no null so this is a collision tile
                            _mapSquares[x, y] = Collision;
                        }
                }
            
        }
        #endregion

        #region Information about Map Squares

        static public int GetSquareByPixelX(int pixelX)
        {
           return pixelX / _tileWidth;
        }

        static public int GetSquareByPixelY(int pixelY)
        {
            return pixelY / _tileHeight;
        }

        static public Vector2 GetSquareAtPixel(Vector2 pixelLocation)
        {
            return new Vector2(
                GetSquareByPixelX((int)pixelLocation.X),
                GetSquareByPixelY((int)pixelLocation.Y));
        }

        static public Vector2 GetSquareCenter(int squareX, int squareY)
        {
            return new Vector2(
                (squareX * _tileWidth) + (_tileWidth / 2),
                (squareY * _tileHeight) + (_tileHeight / 2));
        }

        static public Vector2 GetSquareTop(Vector2 square)
        {
            return new Vector2(
                (square.X * _tileWidth),
                (square.Y * _tileHeight));
        }

        static public Vector2 GetSquareCenter(Vector2 square)
        {
            return GetSquareCenter(
                (int)square.X,
                (int)square.Y);
        }

        static public Rectangle SquareWorldRectangle(int x, int y)
        {
            return new Rectangle(
                x * _tileWidth,
                y * _tileHeight,
                _tileWidth,
                _tileHeight);
        }

        
        #endregion

        #region Information about Map Tiles
        
        //TODO: Find a way to do this
        /*
        public static Thread tiledThread = new Thread(Update);
        private static readonly object _dataLock = new object();
        public static void Update()
        {
            while (!GameManager.GameOver && !GameManager.LevelWon)
            {
                lock (_dataLock)
                {
                    for (int x = 0; x < _map.Width; x++)
                    {
                        for (int y = 0; y < _map.Height; y++)
                        {
                            if (_mapSquares[x, y] != Collision)
                            {
                                for (int i = 0; i < EnemyManager.Enemies.Count; i++)
                                {
                                    Vector2 square = GetSquareAtPixel(EnemyManager.Enemies[i].Center);
                                    if ((int) square.X == x && (int) square.Y == y)
                                    {
                                        _mapSquares[x, y] = ContainsCharacter;
                                    }
                                    else
                                    {
                                        _mapSquares[x, y] = NonCollision;
                                    }

                                }
                                if (_mapSquares[x, y] != ContainsCharacter)
                                {
                                    for (int i = 0; i < PlayerManager.PlayerCharacters.Count; i++)
                                    {
                                        Vector2 square = GetSquareAtPixel(PlayerManager.PlayerCharacters[i].Center);
                                        if ((int) square.X == x && (int) square.Y == y)
                                        {
                                            _mapSquares[x, y] = ContainsCharacter;
                                        }
                                        else
                                        {
                                            _mapSquares[x, y] = NonCollision;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        */
        static public int GetTileAtSquare(int tileX, int tileY)
        {
            return (tileX >= 0) && (tileX < _map.Width) &&
                   (tileY >= 0) && (tileY < _map.Height)
                       ? _mapSquares[tileX, tileY]
                       : -1;
        }

        static public bool IsCollisionTile(int tileX, int tileY)
        {
            int tileIndex = GetTileAtSquare(tileX, tileY);
            if (tileIndex == -1 || tileIndex == NonCollision)
            {
                return false;
            }

            return tileIndex == Collision;
        }

        static public bool IsCollisionTile(Vector2 square)
        {
            return IsCollisionTile((int)square.X, (int)square.Y);
        }

        static public bool IsCollisionTileByPixel(Vector2 pixelLocation)
        {
            return IsCollisionTile(
                GetSquareByPixelX((int)pixelLocation.X),
                GetSquareByPixelY((int)pixelLocation.Y));
        }

        #endregion

        public static Color? GetCollision(Vector2 position)
        {
            var colLayer = _map.GetLayer("Collision");

            var tileLayer = colLayer as TileLayer;

            position.X = (int)position.X;
            position.Y = (int)position.Y;

            var tilePosition = new Vector2((int)(position.X / _tileWidth), (int)(position.Y / _tileHeight));

            if (tileLayer != null)
            {
                var collisionTile = tileLayer.Tiles[(int)tilePosition.X, (int)tilePosition.Y];

                if (collisionTile == null)
                    return null;

                if (collisionTile.Texture != null)
                {
                    int positionOnTileX = ((int)position.X - (((int)position.X / _tileWidth) * _tileWidth));
                    int positionOnTileY = ((int)position.Y - (((int)position.Y / _tileHeight) * _tileHeight));
                    positionOnTileX = (int)MathHelper.Clamp(positionOnTileX, 0, _tileWidth);
                    positionOnTileY = (int)MathHelper.Clamp(positionOnTileY, 0, _tileHeight);

                    int pixelCheckX = (collisionTile.Source.X) + positionOnTileX;
                    int pixelCheckY = (collisionTile.Source.Y) + positionOnTileY;
                    var colDat = new Color[collisionTile.Texture.Height * collisionTile.Texture.Width];
                    collisionTile.Texture.GetData(colDat);
                    return colDat[(pixelCheckY * collisionTile.Texture.Width) + pixelCheckX];
                }
            }
            return null;
        }



        /// <summary>
        /// Checks if any collidable object is present in a map tile 
        /// </summary>
        public static bool GetMapSquareCollision(Rectangle areaToCheck)
        {
            var areaCenter = new Vector2(areaToCheck.X+areaToCheck.Width/2, areaToCheck.Y+areaToCheck.Height/2);
            var worldAreaCenter = areaCenter + GameplayScreen.Camera.Position;

            if (IsCollisionTile(GetSquareAtPixel(worldAreaCenter)))
            {
                return true;
            }
            return false;
            /*
            var colLayer = _map.GetLayer("Collision");
            var tileLayer = colLayer as TileLayer;
            var rectPosition = new Vector2(areaToCheck.X, areaToCheck.Y);
            var worldPosition = rectPosition + GameplayScreen.Camera.Position;
            var collisionRectangle = new Rectangle((int)worldPosition.X, (int)worldPosition.Y, areaToCheck.Width, areaToCheck.Y);
            if (tileLayer != null)
            {
                int x = _map.Width;
                int y = _map.Height;
                for (int i = 0; i < y; i++)
                {
                    for (int j = 0; j < x; j++)
                    {
                        if (tileLayer.Tiles[j, i] != null)
                        {
                            if (tileLayer.Tiles[j, i].Texture != null)
                            {
                                var colTileRect = new Rectangle(j * _tileWidth,
                                                                i * _tileHeight, _tileWidth,
                                                                _tileHeight);
                                if (collisionRectangle.Intersects(colTileRect))
                                {
                                    Debug.WriteLine(colTileRect);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
            */
        }
    }
}
