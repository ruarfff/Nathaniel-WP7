using Microsoft.Xna.Framework;
using NathanielGame.Levels;

namespace NathanielGame
{
    class PathNode
    {
        #region Declarations

        public PathNode ParentNode { get; set; }
        public PathNode EndNode { get; set; }
        private Vector2 _gridLocation;
        public float TotalCost { get; set; }
        public float DirectCost { get; set; }
        #endregion

        #region Properties
        public Vector2 GridLocation
        {
            get { return _gridLocation; }
            set
            {
                _gridLocation = new Vector2(
                    MathHelper.Clamp(value.X, 0f, Level.CurrentMap.Width),
                    MathHelper.Clamp(value.Y, 0f, Level.CurrentMap.Height));
            }
        }

        public int GridX
        {
            get { return (int)_gridLocation.X; }
        }

        public int GridY
        {
            get { return (int)_gridLocation.Y; }
        }
        #endregion

        #region Constructor
        public PathNode(
            PathNode parentNode,
            PathNode endNode,
            Vector2 gridLocation,
            float cost)
        {
            ParentNode = parentNode;
            GridLocation = gridLocation;
            EndNode = endNode;
            DirectCost = cost;
            if (endNode != null)
            {
                TotalCost = DirectCost + LinearCost();
            }
        }
        #endregion

        #region Helper Methods
        public float LinearCost()
        {
            return (
                Vector2.Distance(
                EndNode.GridLocation,
                GridLocation));
        }
        #endregion

        #region Public Methods
        public bool IsEqualToNode(PathNode node)
        {
            return (GridLocation == node.GridLocation);
        }
        #endregion
    }
}
