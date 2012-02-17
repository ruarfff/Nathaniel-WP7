#region File Description

//////////////////////////////////////////////////////////////////////////
// File     :   "BasicPrimitives.cs"
// Author   :   Jeffrey Feck
// Version  :   XNA 4.0 
// Purpose  :   Contains BasicPrimitives data members and function implementations.
//
// Special Thanks : http://forums.xna.com/forums/t/7414.aspx
//////////////////////////////////////////////////////////////////////////

#endregion // File Description

#region Using Statements

//////////////////////////////////////////////////////////////////////////
// System libraries.
using System;
using System.Collections.Generic; // List.

// XNA libraries.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; // GraphicsDevice.

// Other libraries.
using Triangulator;
//
//////////////////////////////////////////////////////////////////////////

#endregion // Using Statments

namespace NathanielGame
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Derives : None</para>
    /// <para>Purpose : Render 2D shapes.</para>
    /// <para>Version : 1.0</para>
    /// </summary>
    //////////////////////////////////////////////////////////////////////////
    public class BasicPrimitives
    {
        /*********************************************************************/
        // Members.
        /*********************************************************************/

        #region Enumerations

        /// <summary>Spline Interpolations.</summary>
        public enum Spline
        {
            Linear,
            Cosine,
            Cubic,
            Hermite
        }

        #endregion // Enumerations

        #region Fields

        #region 2D Members

        /// <summary>The outline color of the primitive object.</summary>
        private Color m_OutlineColor = Color.White;

        /// <summary>The fill in color of the primitive object.</summary>
        private Color m_FillInColor = Color.White;

        /// <summary>The position of the primitive object.</summary>
        private Vector2 m_vPosition = Vector2.Zero;

        /// <summary>The center position of the primitive object</summary>
        private Vector2 m_vCentriod = Vector2.Zero;

        /// <summary>The render depth of the primitive line object (0 = front, 1 = back).</summary>
        private float m_fDepth = 0f;

        /// <summary>The thickness of the shape's edge.</summary>
        private float m_fThickness = 1f;

        /// <summary>1x1 pixel that creates the shape.</summary>
        private Texture2D m_Pixel = null;

        /// <summary>List of vectors.</summary>
        private List<Vector2> m_VectorList = new List<Vector2>();

        #endregion // 2D Members

        #region 3D Members

        /// <summary>Graphics device handle.</summary>
        private GraphicsDevice m_GraphicsDevice = null;

        /// <summary>Used to set states for rendering.</summary>
        private RasterizerState m_RasterizerState = new RasterizerState();

        /// <summary>Effect handle.</summary>
        private BasicEffect m_Effect = null;

        /// <summary>Vertex buffer.</summary>
        private VertexBuffer m_VertexBuffer = null;

        /// <summary>Index buffer.</summary>
        private IndexBuffer m_IndexBuffer = null;

        /// <summary>World matrix used for custom shadeer.</summary>
        private Matrix m_World = Matrix.Identity;

        /// <summary>View matrix used for custom shader.</summary>
        private Matrix m_View = Matrix.Identity;

        /// <summary>Projection matrix used for custom shader.</summary>
        private Matrix m_Projection = Matrix.Identity;

        /// <summary>Scale matrix.</summary>
        private Matrix m_matScale = Matrix.Identity;

        /// <summary>Accumulated rotational value.</summary>
        private float m_fRotation = 0f;

        /// <summary>Used to calculate the scale matrix.</summary>
        private float m_fScaleOffsetSpeed = 0.043f;

        /// <summary>Used to calculate precise rotation from 2D to 3D.</summary>
        private float m_fRotationOffset = 0.015f;

        /// <summary>Number of vertices.</summary>
        private int m_nVertices = 0;

        /// <summary>Number of primitives.</summary>
        private int m_nPrimitives = 0;

        #endregion // 3D Members

        #endregion // Fields

        #region Properties

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Set both the outline and fill in colour of the primitive object.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        public Color Colour
        {
            set { m_OutlineColor = value; m_FillInColor = value; }
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Get/Set the outline colour of the primitive object.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        public Color OutlineColour
        {
            get { return m_OutlineColor; }
            set { m_OutlineColor = value; }
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Get/Set the fill in colour of the primitive object.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        public Color FillInColour
        {
            get { return m_FillInColor; }
            set { m_FillInColor = value; }
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Get/Set the position of the primitive object.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        public Vector2 Position
        {
            get { return m_vPosition; }
            set
            {
                //////////////////////////////////////////////////////////////////////////
                // Set 2D position.
                m_vPosition = value;

                // The outline shape draws by default [0, 0] from the top left, while the 
                // fill in shape draws by default at the center [0, 0, 0] of the screen 
                // in world space. So we need to calculate the middle of the screen, then 
                // subtract the position, unproject into world space, reverse [X, Y] coordinates,
                // clamp Z to zero, and set it to the world matrix.
                Vector2 vScreenOffset = new Vector2(m_GraphicsDevice.Viewport.Width >> 1, m_GraphicsDevice.Viewport.Height >> 1);
                Vector3 vTranslation = m_GraphicsDevice.Viewport.Unproject(new Vector3(vScreenOffset - value, 0f), m_Projection, m_View, Matrix.Identity);
                vTranslation.X *= -1f;
                vTranslation.Y *= -1f;
                vTranslation.Z = 0f;
                m_World.Translation = vTranslation;
                //
                //////////////////////////////////////////////////////////////////////////
            }
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Get/Set the render depth of the primitive line object (0 = front, 1 = back).
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        public float Depth
        {
            get { return m_fDepth; }
            set { m_fDepth = value; }
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Get/Set the thickness of the shape's edge.
        /// <para>NOTE: The value is automatically clamped between 1 and float.MaxValue</para>
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        public float Thickness
        {
            get { return m_fThickness; }
            set
            {
                // Clamp the thickness value.
                m_fThickness = MathHelper.Clamp(value, 1f, float.MaxValue);

                // Calculate scale offset. We do this since scaling in 2D is based on pixels,
                // while scaling in 3D is based on matrices. Precision is based on the offset
                // speed. The higher the value, the slower the scale and vise versa.
                float fScaleOffset = (m_fThickness - 1.0f) / ((m_fThickness * (m_fScaleOffsetSpeed * m_fThickness) + 20f));

                // Calculate orign and scale from screen space to world space.
                Vector3 vOrigin = m_GraphicsDevice.Viewport.Unproject(new Vector3(m_vCentriod, 0f), m_Projection, m_View, Matrix.Identity);
                Vector3 vScale = fScaleOffset * m_GraphicsDevice.Viewport.Unproject(new Vector3(-(float)Math.Pow(m_fThickness, 2), -(float)Math.Pow(m_fThickness, 2), 0f), m_Projection, m_View, Matrix.Identity);

                // Store scale matrix based on "Scale From Origin" algorithm.
                // Final Result = -Orign[XYZ] * Scale * Orign[XYZ]
                m_matScale = Matrix.CreateTranslation(-vOrigin) * Matrix.CreateScale(new Vector3(1f + -vScale.X, 1f + vScale.Y, 1f)) * Matrix.CreateTranslation(vOrigin);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the number of vectors which make up the primitive object.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        public int CountVectors
        {
            get { return m_VectorList.Count; }
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the vector position from the list.
        /// </summary>
        /// <param name="_nIndex">The index to get from.</param>
        //////////////////////////////////////////////////////////////////////////
        public Vector2 GetVector(int _nIndex)
        {
            return m_VectorList[_nIndex];
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Get the center average position of the primitive object.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        public Vector2 Centroid
        {
            get { return m_vCentriod; }
        }

        #endregion // Properties

        /*********************************************************************/
        // Functions.
        /*********************************************************************/

        #region Initialization | Dispose

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates a new primitive object.
        /// </summary>
        /// <param name="_graphicsDevice">The graphics device object to use.</param>
        //////////////////////////////////////////////////////////////////////////
        public BasicPrimitives(GraphicsDevice _graphicsDevice)
        {
            //////////////////////////////////////////////////////////////////////////
            // Set members.
            m_GraphicsDevice = _graphicsDevice;

            // Create the pixel texture.
            m_Pixel = new Texture2D(_graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            m_Pixel.SetData<Color>(new Color[] { Color.White });

            /// Setup view and projection matrix.
            m_View = Matrix.CreateLookAt(Vector3.UnitZ, -Vector3.UnitZ, Vector3.Up);
            m_Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, m_GraphicsDevice.Viewport.AspectRatio, 0.1f, 1.0f);

            // Setup basic effect.
            m_Effect = new BasicEffect(_graphicsDevice);
            Vector3 vPoint = m_GraphicsDevice.Viewport.Unproject(new Vector3(new Vector2(m_GraphicsDevice.Viewport.Width / 2, m_GraphicsDevice.Viewport.Height / 2), 0f), m_Projection, m_View, Matrix.Identity);
            vPoint.Z = 0f;
            m_World.Translation = vPoint;
            m_Effect.World = m_World;
            m_Effect.View = m_View;
            m_Effect.Projection = m_Projection;
            m_Effect.VertexColorEnabled = true;

            // Setup rasterizer states.
            m_RasterizerState.CullMode = CullMode.None;
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called when the primitive object is destroyed.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        ~BasicPrimitives()
        {
            if (m_IndexBuffer != null) m_IndexBuffer.Dispose();
            if (m_VertexBuffer != null) m_VertexBuffer.Dispose();
            m_Effect.Dispose();
            m_Pixel.Dispose();
            m_VectorList.Clear();
        }

        #endregion // Initialization | Dispose

        #region List Manipulation Methods

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a vector to the primitive object.
        /// </summary>
        /// <param name="_vPosition">The vector to add.</param>
        //////////////////////////////////////////////////////////////////////////
        public void AddVector(Vector2 _vPosition)
        {
            m_VectorList.Add(_vPosition);
            m_vCentriod = CalculateCentroid();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Inserts a vector into the primitive object.
        /// </summary>
        /// <param name="_nIndex">The index to insert it at.</param>
        /// <param name="_vPosition">The vector to insert.</param>
        //////////////////////////////////////////////////////////////////////////
        public void InsertVector(int _nIndex, Vector2 _vPosition)
        {
            m_VectorList.Insert(_nIndex, _vPosition);
            m_vCentriod = CalculateCentroid();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes a vector from the primitive object.
        /// </summary>
        /// <param name="_vPosition">The vector to remove.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RemoveVector(Vector2 _vPosition)
        {
            m_VectorList.Remove(_vPosition);
            m_vCentriod = CalculateCentroid();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes a vector from the primitive object.
        /// </summary>
        /// <param name="_nIndex">The index of the vector to remove.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RemoveVector(int _nIndex)
        {
            m_VectorList.RemoveAt(_nIndex);
            m_vCentriod = CalculateCentroid();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clears all vectors from the list.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        public void ClearVectors()
        {
            m_VectorList.Clear();
            m_vCentriod = Vector2.Zero;
        }

        #endregion // List Manipulation Methods

        #region Creation Methods

        //////////////////////////////////////////////////////////////////////////
        /// <summary> 
        /// Create a line primitive.
        /// </summary>
        /// <param name="_vStart">Start of the line, in pixels.</param>
        /// <param name="_vEnd">End of the line, in pixels.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CreateLine(Vector2 _vStart, Vector2 _vEnd)
        {
            m_VectorList.Clear();
            m_VectorList.Add(_vStart);
            m_VectorList.Add(_vEnd);
            m_vCentriod = CalculateCentroid();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Create a triangle primitive.
        /// </summary>
        /// <param name="_vPoint1">Fist point, in pixels.</param>
        /// <param name="_vPoint2">Second point, in pixels.</param>
        /// <param name="_vPoint3">Third point, in pixels.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CreateTriangle(Vector2 _vPoint1, Vector2 _vPoint2, Vector2 _vPoint3)
        {
            m_VectorList.Clear();
            m_VectorList.Add(_vPoint1);
            m_VectorList.Add(_vPoint2);
            m_VectorList.Add(_vPoint3);
            m_VectorList.Add(_vPoint1);
            m_vCentriod = CalculateCentroid();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Create a square primitive.
        /// </summary>
        /// <param name="_vTopLeft">Top left hand corner of the square.</param>
        /// <param name="_vBottomRight">Bottom right hand corner of the square.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CreateSquare(Vector2 _vTopLeft, Vector2 _vBottomRight)
        {
            m_VectorList.Clear();
            m_VectorList.Add(_vTopLeft);
            m_VectorList.Add(new Vector2(_vTopLeft.X, _vBottomRight.Y));
            m_VectorList.Add(_vBottomRight);
            m_VectorList.Add(new Vector2(_vBottomRight.X, _vTopLeft.Y));
            m_VectorList.Add(_vTopLeft);
            m_vCentriod = CalculateCentroid();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates a circle starting from (0, 0).
        /// </summary>
        /// <param name="_fRadius">The radius (half the width) of the circle.</param>
        /// <param name="_nSides">The number of sides on the circle. (64 is average).</param>
        //////////////////////////////////////////////////////////////////////////
        public void CreateCircle(float _fRadius, int _nSides)
        {
            m_VectorList.Clear();

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            float fMax = (float)MathHelper.TwoPi;
            float fStep = fMax / (float)_nSides;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create the full circle.
            for (float fTheta = fMax; fTheta >= -1; fTheta -= fStep)
            {
                m_VectorList.Add(new Vector2(_fRadius * (float)Math.Cos((double)fTheta),
                                             _fRadius * (float)Math.Sin((double)fTheta)));
            }
            //
            //////////////////////////////////////////////////////////////////////////

            // Calculate center average position.
            m_vCentriod = CalculateCentroid();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Creates an ellipse starting from (0, 0) with the given width and height.
        /// Vectors are generated using the parametric equation of an ellipse.
        /// </summary>
        /// <param name="_fSemiMajorAxis">The width of the ellipse at its center.</param>
        /// <param name="_fSemiMinorAxis">The height of the ellipse at its center.</param>
        /// <param name="_nSides">The number of sides on the ellipse. (64 is average).</param>
        //////////////////////////////////////////////////////////////////////////
        public void CreateEllipse(float _fSemiMajorAxis, float _fSemiMinorAxis, int _nSides)
        {
            m_VectorList.Clear();

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            float fMax = (float)MathHelper.TwoPi;
            float fStep = fMax / (float)_nSides;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create full ellipse.
            for (float fTheta = fMax; fTheta >= -1; fTheta -= fStep)
            {
                m_VectorList.Add(new Vector2((float)(_fSemiMajorAxis * Math.Cos(fTheta)),
                                             (float)(_fSemiMinorAxis * Math.Sin(fTheta))));
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Create a linear spline based on 4 point positions.
        /// </summary>
        /// <param name="_vPoint1">First position.</param>
        /// <param name="_vPoint2">Second position.</param>
        /// <param name="_vPoint3">Third position.</param>
        /// <param name="_vPoint4">Fourth position.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CreateLinearSpline(Vector2 _vPoint1, Vector2 _vPoint2, Vector2 _vPoint3, Vector2 _vPoint4)
        {
            CalculateSpline(Spline.Linear, new Vector2[] { _vPoint1, _vPoint2, _vPoint3, _vPoint4 }, 0f, 0f);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Create a cosine spline based on 4 point positions.
        /// </summary>
        /// <param name="_vPoint1">First position.</param>
        /// <param name="_vPoint2">Second position.</param>
        /// <param name="_vPoint3">Third position.</param>
        /// <param name="_vPoint4">Fourth position.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CreateCosineSpline(Vector2 _vPoint1, Vector2 _vPoint2, Vector2 _vPoint3, Vector2 _vPoint4)
        {
            CalculateSpline(Spline.Cosine, new Vector2[] { _vPoint1, _vPoint2, _vPoint3, _vPoint4 }, 0f, 0f);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Create a cubic spline based on 4 point positions.
        /// </summary>
        /// <param name="_vPoint1">First position.</param>
        /// <param name="_vPoint2">Second position.</param>
        /// <param name="_vPoint3">Third position.</param>
        /// <param name="_vPoint4">Fourth position.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CreateCubicSpline(Vector2 _vPoint1, Vector2 _vPoint2, Vector2 _vPoint3, Vector2 _vPoint4)
        {
            CalculateSpline(Spline.Cubic, new Vector2[] { _vPoint1, _vPoint2, _vPoint3, _vPoint4 }, 0f, 0f);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Create a hermite spline based on 4 point positions and modifications.
        /// </summary>
        /// <param name="_vPoint1">First position.</param>
        /// <param name="_vPoint2">Second position.</param>
        /// <param name="_vPoint3">Third position.</param>
        /// <param name="_vPoint4">Fourth position.</param>
        /// <param name="_fTension">Tension allows to tighten up the curvature. 1 (or greater) = high tension, 0 = normal tension, -1 (or lower) = low tension.</param>
        /// <param name="_fBias">Bias allows to twist the curve. 1 (or greater) = Towards first segment, 0 = even, -1 (or lower) = Towards end segment.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CreateHermiteSpline(Vector2 _vPoint1, Vector2 _vPoint2, Vector2 _vPoint3, Vector2 _vPoint4,
                                        float _fTension, float _fBias)
        {
            CalculateSpline(Spline.Hermite, new Vector2[] { _vPoint1, _vPoint2, _vPoint3, _vPoint4 }, _fTension, _fBias);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Create a bézier spline based on 4 point positions.
        /// </summary>
        /// <param name="_vPoint1">First position.</param>
        /// <param name="_vPoint2">Second position.</param>
        /// <param name="_vPoint3">Third position.</param>
        /// <param name="_vPoint4">Fourth position.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CreateBezierSpline(Vector2 _vPoint1, Vector2 _vPoint2, Vector2 _vPoint3, Vector2 _vPoint4)
        {
            BezierSpline(_vPoint1, _vPoint2, _vPoint3, _vPoint4);
        }

        #endregion // Creation Methods

        #region Render Methods

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render the center average point of the primitive.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        /// <param name="_Color">Color of the point.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderCentriod(SpriteBatch _spriteBatch)
        {
            // Stretch the pixel between the two vectors.
            _spriteBatch.Draw(m_Pixel,
                              m_vCentriod,
                              null,
                              m_OutlineColor,
                              0f,
                              new Vector2(0.5f, 0.5f),
                              m_fThickness,
                              SpriteEffects.None,
                              m_fDepth);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render the center average point of the primitive.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        /// <param name="_Color">Color of the point.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderCentriod(SpriteBatch _spriteBatch, Color _Color)
        {
            // Stretch the pixel between the two vectors.
            _spriteBatch.Draw(m_Pixel,
                              m_vCentriod,
                              null,
                              _Color,
                              0f,
                              new Vector2(0.5f, 0.5f),
                              m_fThickness,
                              SpriteEffects.None,
                              m_fDepth);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render the center average point of the primitive.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        /// <param name="_fThickness">The thickness of the point in pixels.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderCentriod(SpriteBatch _spriteBatch, float _fThickness)
        {
            // Stretch the pixel between the two vectors.
            _spriteBatch.Draw(m_Pixel,
                              m_vCentriod,
                              null,
                              m_OutlineColor,
                              0f,
                              new Vector2(0.5f, 0.5f),
                              _fThickness,
                              SpriteEffects.None,
                              m_fDepth);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render the center average point of the primitive.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        /// <param name="_Color">Color of the point.</param>
        /// <param name="_fThickness">The thickness of the point in pixels.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderCentriod(SpriteBatch _spriteBatch, Color _Color, float _fThickness)
        {
            // Stretch the pixel between the two vectors.
            _spriteBatch.Draw(m_Pixel,
                              m_vCentriod,
                              null,
                              _Color,
                              0f,
                              new Vector2(0.5f, 0.5f),
                              _fThickness,
                              SpriteEffects.None,
                              m_fDepth);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render the center average point of the primitive.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        /// <param name="_Color">Color of the point.</param>
        /// <param name="_fThickness">The thickness of the point in pixels.</param>
        /// <param name="_fDepth">The render depth of the primitive line object (0 = front, 1 = back).</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderCentriod(SpriteBatch _spriteBatch, Color _Color, float _fThickness, float _fDepth)
        {
            // Stretch the pixel between the two vectors.
            _spriteBatch.Draw(m_Pixel,
                              m_vCentriod,
                              null,
                              _Color,
                              0f,
                              new Vector2(0.5f, 0.5f),
                              _fThickness,
                              SpriteEffects.None,
                              _fDepth);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render points of the primitive.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderPointPrimitive(SpriteBatch _spriteBatch)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count <= 0)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
            float fAngle = 0f;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of vectors.
            for (int i = m_VectorList.Count - 1; i >= 1; --i)
            {
                // Store positions.
                vPosition1 = m_VectorList[i - 1];
                vPosition2 = m_VectorList[i];

                // Calculate the angle between the two vectors.
                fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
                                           (double)(vPosition2.X - vPosition1.X));

                // Stretch the pixel between the two vectors.
                _spriteBatch.Draw(m_Pixel,
                                  m_vPosition + m_VectorList[i],
                                  null,
                                  m_OutlineColor,
                                  fAngle,
                                  new Vector2(0.5f, 0.5f),
                                  m_fThickness,
                                  SpriteEffects.None,
                                  m_fDepth);
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render points of the primitive.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        /// <param name="_fAngle">The rotation in radians. (0.0f is default).</param>
        /// <param name="_vPivot">Position in which to rotate around.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderPointPrimitive(SpriteBatch _spriteBatch, float _fAngle, Vector2 _vPivot)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count <= 0)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Rotate object based on pivot.
            Rotate(_fAngle, _vPivot);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
            float fAngle = 0f;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of vectors.
            for (int i = m_VectorList.Count - 1; i >= 1; --i)
            {
                // Store positions.
                vPosition1 = m_VectorList[i - 1];
                vPosition2 = m_VectorList[i];

                // Calculate the angle between the two vectors.
                fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
                                           (double)(vPosition2.X - vPosition1.X));

                // Stretch the pixel between the two vectors.
                _spriteBatch.Draw(m_Pixel,
                                  m_vPosition + m_VectorList[i],
                                  null,
                                  m_OutlineColor,
                                  fAngle,
                                  new Vector2(0.5f, 0.5f),
                                  m_fThickness,
                                  SpriteEffects.None,
                                  m_fDepth);
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render the lines of the primitive.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderLinePrimitive(SpriteBatch _spriteBatch)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count < 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
            float fDistance = 0f, fAngle = 0f;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of vectors.
            for (int i = m_VectorList.Count - 1; i >= 1; --i)
            {
                // Store positions.
                vPosition1 = m_VectorList[i - 1];
                vPosition2 = m_VectorList[i];

                // Calculate the distance between the two vectors.
                fDistance = Vector2.Distance(vPosition1, vPosition2);

                // Calculate the angle between the two vectors.
                fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
                                           (double)(vPosition2.X - vPosition1.X));

                // Stretch the pixel between the two vectors.
                _spriteBatch.Draw(m_Pixel,
                                  m_vPosition + vPosition1,
                                  null,
                                  m_OutlineColor,
                                  fAngle,
                                  new Vector2(0, 0.5f),
                                  new Vector2(fDistance, m_fThickness),
                                  SpriteEffects.None,
                                  m_fDepth);
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render the lines of the primitive.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        /// <param name="_fAngle">The rotation in radians. (0.0f is default).</param>
        /// <param name="_vPivot">Position in which to rotate around.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderLinePrimitive(SpriteBatch _spriteBatch, float _fAngle, Vector2 _vPivot)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count < 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Rotate object based on pivot.
            Rotate(_fAngle, _vPivot);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
            float fDistance = 0f, fAngle = 0f;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of vectors.
            for (int i = m_VectorList.Count - 1; i >= 1; --i)
            {
                // Store positions.
                vPosition1 = m_VectorList[i - 1];
                vPosition2 = m_VectorList[i];

                // Calculate the distance between the two vectors.
                fDistance = Vector2.Distance(vPosition1, vPosition2);

                // Calculate the angle between the two vectors.
                fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
                                           (double)(vPosition2.X - vPosition1.X));

                // Stretch the pixel between the two vectors.
                _spriteBatch.Draw(m_Pixel,
                                  m_vPosition + vPosition1,
                                  null,
                                  m_OutlineColor,
                                  fAngle,
                                  new Vector2(0, 0.5f),
                                  new Vector2(fDistance, m_fThickness),
                                  SpriteEffects.None,
                                  m_fDepth);
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render primitive by using a square algorithm.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderSquarePrimitive(SpriteBatch _spriteBatch)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count < 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero, vLength = Vector2.Zero;
            float fDistance = 0f, fAngle = 0f;
            int nCount = 0;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of vectors.
            for (int i = m_VectorList.Count - 1; i >= 1; --i)
            {
                //////////////////////////////////////////////////////////////////////////
                // Store positions.
                vPosition1 = m_VectorList[i - 1];
                vPosition2 = m_VectorList[i];
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Calculate the distance between the two vectors.
                fDistance = Vector2.Distance(vPosition1, vPosition2);

                // Calculate the angle between the two vectors.
                fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
                                           (double)(vPosition2.X - vPosition1.X));

                // Calculate length.
                vLength = vPosition2 - vPosition1;
                vLength.Normalize();

                // Calculate count for roundness.
                nCount = (int)Math.Round(fDistance);
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Run through and render the primitive.
                while (nCount-- > 0)
                {
                    // Increment position.
                    vPosition1 += vLength;

                    // Stretch the pixel between the two vectors.
                    _spriteBatch.Draw(m_Pixel,
                                      m_vPosition + vPosition1,
                                      null,
                                      m_OutlineColor,
                                      0,
                                      Vector2.Zero,
                                      m_fThickness,
                                      SpriteEffects.None,
                                      m_fDepth);
                }
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render primitive by using a square algorithm.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        /// <param name="_fAngle">The rotation in radians. (0.0f is default).</param>
        /// <param name="_vPivot">Position in which to rotate around.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderSquarePrimitive(SpriteBatch _spriteBatch, float _fAngle, Vector2 _vPivot)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count < 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Rotate object based on pivot.
            Rotate(_fAngle, _vPivot);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero, vLength = Vector2.Zero;
            float fDistance = 0f, fAngle = 0f;
            int nCount = 0;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of vectors.
            for (int i = m_VectorList.Count - 1; i >= 1; --i)
            {
                //////////////////////////////////////////////////////////////////////////
                // Store positions.
                vPosition1 = m_VectorList[i - 1];
                vPosition2 = m_VectorList[i];
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Calculate the distance between the two vectors.
                fDistance = Vector2.Distance(vPosition1, vPosition2);

                // Calculate the angle between the two vectors.
                fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
                                           (double)(vPosition2.X - vPosition1.X));

                // Calculate length.
                vLength = vPosition2 - vPosition1;
                vLength.Normalize();

                // Calculate count for roundness.
                nCount = (int)Math.Round(fDistance);
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Run through and render the primitive.
                while (nCount-- > 0)
                {
                    // Increment position.
                    vPosition1 += vLength;

                    // Stretch the pixel between the two vectors.
                    _spriteBatch.Draw(m_Pixel,
                                      m_vPosition + vPosition1,
                                      null,
                                      m_OutlineColor,
                                      0,
                                      Vector2.Zero,
                                      m_fThickness,
                                      SpriteEffects.None,
                                      m_fDepth);
                }
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render primitive by using a round algorithm.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderRoundPrimitive(SpriteBatch _spriteBatch)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count < 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero, vLength = Vector2.Zero;
            float fDistance = 0f, fAngle = 0f;
            int nCount = 0;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of vectors.
            for (int i = m_VectorList.Count - 1; i >= 1; --i)
            {
                //////////////////////////////////////////////////////////////////////////
                // Store positions.
                vPosition1 = m_VectorList[i - 1];
                vPosition2 = m_VectorList[i];
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Calculate the distance between the two vectors.
                fDistance = Vector2.Distance(vPosition1, vPosition2);

                // Calculate the angle between the two vectors.
                fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
                                           (double)(vPosition2.X - vPosition1.X));

                // Calculate length.
                vLength = vPosition2 - vPosition1;
                vLength.Normalize();

                // Calculate count for roundness.
                nCount = (int)Math.Round(fDistance);
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Run through and render the primitive.
                while (nCount-- > 0)
                {
                    // Increment position.
                    vPosition1 += vLength;

                    // Stretch the pixel between the two vectors.
                    _spriteBatch.Draw(m_Pixel,
                                      m_vPosition + vPosition1 + 0.5f * (vPosition2 - vPosition1),
                                      null,
                                      m_OutlineColor,
                                      fAngle,
                                      new Vector2(0.5f, 0.5f),
                                      new Vector2(fDistance, m_fThickness),
                                      SpriteEffects.None,
                                      m_fDepth);
                }
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render primitive by using a round algorithm.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        /// <param name="_fAngle">The rotation in radians. (0.0f is default).</param>
        /// <param name="_vPivot">Position in which to rotate around.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderRoundPrimitive(SpriteBatch _spriteBatch, float _fAngle, Vector2 _vPivot)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count < 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Rotate object based on pivot.
            Rotate(_fAngle, _vPivot);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero, vLength = Vector2.Zero;
            float fDistance = 0f, fAngle = 0f;
            int nCount = 0;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of vectors.
            for (int i = m_VectorList.Count - 1; i >= 1; --i)
            {
                //////////////////////////////////////////////////////////////////////////
                // Store positions.
                vPosition1 = m_VectorList[i - 1];
                vPosition2 = m_VectorList[i];
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Calculate the distance between the two vectors.
                fDistance = Vector2.Distance(vPosition1, vPosition2);

                // Calculate the angle between the two vectors.
                fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
                                           (double)(vPosition2.X - vPosition1.X));

                // Calculate length.
                vLength = vPosition2 - vPosition1;
                vLength.Normalize();

                // Calculate count for roundness.
                nCount = (int)Math.Round(fDistance);
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Run through and render the primitive.
                while (nCount-- > 0)
                {
                    // Increment position.
                    vPosition1 += vLength;

                    // Stretch the pixel between the two vectors.
                    _spriteBatch.Draw(m_Pixel,
                                      m_vPosition + vPosition1 + 0.5f * (vPosition2 - vPosition1),
                                      null,
                                      m_OutlineColor,
                                      fAngle,
                                      new Vector2(0.5f, 0.5f),
                                      new Vector2(fDistance, m_fThickness),
                                      SpriteEffects.None,
                                      m_fDepth);
                }
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render primitive by using a point and line algorithm.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderPolygonPrimitive(SpriteBatch _spriteBatch)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count < 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
            float fDistance = 0f, fAngle = 0f;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of vectors.
            for (int i = m_VectorList.Count - 1; i >= 1; --i)
            {
                //////////////////////////////////////////////////////////////////////////
                // Store positions.
                vPosition1 = m_VectorList[i - 1];
                vPosition2 = m_VectorList[i];
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Calculate the distance between the two vectors.
                fDistance = Vector2.Distance(vPosition1, vPosition2);

                // Calculate the angle between the two vectors.
                fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
                                           (double)(vPosition2.X - vPosition1.X));
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Stretch the pixel between the two vectors.
                _spriteBatch.Draw(m_Pixel,
                                  Position + vPosition1 + 0.5f * (vPosition2 - vPosition1),
                                  null,
                                  m_OutlineColor,
                                  fAngle,
                                  new Vector2(0.5f, 0.5f),
                                  new Vector2(fDistance, Thickness),
                                  SpriteEffects.None,
                                  m_fDepth);

                // Render the points of the polygon.
                _spriteBatch.Draw(m_Pixel,
                                  m_vPosition + vPosition1,
                                  null,
                                  m_OutlineColor,
                                  fAngle,
                                  new Vector2(0.5f, 0.5f),
                                  m_fThickness,
                                  SpriteEffects.None,
                                  m_fDepth);
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render primitive by using a point and line algorithm.
        /// </summary>
        /// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
        /// <param name="_fAngle">The rotation in radians. (0.0f is default).</param>
        /// <param name="_vPivot">Position in which to rotate around.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderPolygonPrimitive(SpriteBatch _spriteBatch, float _fAngle, Vector2 _vPivot)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count < 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Rotate object based on pivot.
            Rotate(_fAngle, _vPivot);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
            float fDistance = 0f, fAngle = 0f;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of vectors.
            for (int i = m_VectorList.Count - 1; i >= 1; --i)
            {
                //////////////////////////////////////////////////////////////////////////
                // Store positions.
                vPosition1 = m_VectorList[i - 1];
                vPosition2 = m_VectorList[i];
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Calculate the distance between the two vectors.
                fDistance = Vector2.Distance(vPosition1, vPosition2);

                // Calculate the angle between the two vectors.
                fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
                                           (double)(vPosition2.X - vPosition1.X));
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Stretch the pixel between the two vectors.
                _spriteBatch.Draw(m_Pixel,
                                  Position + vPosition1 + 0.5f * (vPosition2 - vPosition1),
                                  null,
                                  m_OutlineColor,
                                  fAngle,
                                  new Vector2(0.5f, 0.5f),
                                  new Vector2(fDistance, Thickness),
                                  SpriteEffects.None,
                                  m_fDepth);

                // Render the points of the polygon.
                _spriteBatch.Draw(m_Pixel,
                                  m_vPosition + vPosition1,
                                  null,
                                  m_OutlineColor,
                                  fAngle,
                                  new Vector2(0.5f, 0.5f),
                                  m_fThickness,
                                  SpriteEffects.None,
                                  m_fDepth);
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render the polygon using vertices.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        public void RenderFillInPolygon()
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_nVertices <= 0 || m_nPrimitives <= 0)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Set rasterizer states.
            m_GraphicsDevice.RasterizerState = m_RasterizerState;

            // Set graphic device members.
            m_GraphicsDevice.Indices = m_IndexBuffer;
            m_GraphicsDevice.SetVertexBuffer(m_VertexBuffer);
            //
            ////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////
            // Begin effect.
            foreach (EffectPass pass in m_Effect.CurrentTechnique.Passes)
            {
                // Begins the pass.
                pass.Apply();

                // Apply world matrix (scale, rotation, translation).
                m_Effect.World = m_matScale * m_World;

                // Render the list of triangles.
                m_GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_nVertices, 0, m_nPrimitives);
            }
            //
            ////////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Render the polygon using vertices.
        /// </summary>
        /// <param name="_fAngle">The rotation in radians. (0.0f is default).</param>
        /// <param name="_vPivot">Position in which to rotate around.</param>
        //////////////////////////////////////////////////////////////////////////
        public void RenderFillInPolygon(float _fAngle, Vector2 _vPivot)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_nVertices <= 0 || m_nPrimitives <= 0)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Rotate object based on pivot.
            Rotate(_fAngle, _vPivot);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Set rasterizer states.
            m_GraphicsDevice.RasterizerState = m_RasterizerState;

            // Set graphic device members.
            m_GraphicsDevice.Indices = m_IndexBuffer;
            m_GraphicsDevice.SetVertexBuffer(m_VertexBuffer);
            //
            ////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////
            // Begin effect.
            foreach (EffectPass pass in m_Effect.CurrentTechnique.Passes)
            {
                // Begins the pass.
                pass.Apply();

                // Apply world matrix (scale, rotation, translation).
                m_Effect.World = m_matScale * m_World;

                // Render the list of triangles.
                m_GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_nVertices, 0, m_nPrimitives);
            }
            //
            ////////////////////////////////////////////////////////////////////////////
        }

        #endregion // Render Methods

        #region Public Methods

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Rotate primitive object based on pivot.
        /// </summary>
        /// <param name="_fAngle">The rotation in radians. (0.0f is default).</param>
        /// <param name="_vPivot">Position in which to rotate around.</param>
        //////////////////////////////////////////////////////////////////////////
        public void Rotate(float _fAngle, Vector2 _vPivot)
        {
            //////////////////////////////////////////////////////////////////////////
            // Accumulate rotational value.
            m_fRotation += _fAngle;

            // Clamp overflow.
            if (m_fRotation > MathHelper.TwoPi)
                m_fRotation -= MathHelper.TwoPi;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create rotation matrix for both 2D pixels and 3D vertices.
            Matrix matRotation2D = Matrix.CreateRotationZ(_fAngle);
            Matrix matRotation3D = Matrix.CreateRotationZ(-m_fRotation - m_fRotationOffset);

            // Run through the vector list, and apply the "Rotate 2D Around 2D Pivot" algorithm.
            // Final Result = -Pivot[XYZ] * Rotation * Pivot[XYZ]
            for (int i = m_VectorList.Count - 1; i >= 0; --i)
                m_VectorList[i] = Vector2.Transform(m_VectorList[i] - _vPivot, matRotation2D) + _vPivot;

            // The outline shape draws by default [0, 0] from the top left, while the 
            // fill in shape draws by default at the center [0, 0, 0] of the screen 
            // in world space. So we need to calculate the middle of the screen, then 
            // subtract the position, unproject into world space, reverse [X, Y] coordinates,
            // clamp Z to zero, and set it to the world matrix.
            Vector2 vScreenOffset = new Vector2(m_GraphicsDevice.Viewport.Width >> 1, m_GraphicsDevice.Viewport.Height >> 1);
            Vector3 vTranslation = m_GraphicsDevice.Viewport.Unproject(new Vector3(vScreenOffset - m_vPosition, 0f), m_Projection, m_View, Matrix.Identity);
            vTranslation.X *= -1f;
            vTranslation.Y *= -1f;
            vTranslation.Z = 0f;

            // Calculate pivot from screen space to world space.
            Vector3 vUnprojectPivot = m_GraphicsDevice.Viewport.Unproject(new Vector3(_vPivot, 0f), m_Projection, m_View, Matrix.Identity);
            Vector3 vOrigin = m_GraphicsDevice.Viewport.Unproject(new Vector3(m_vCentriod, 0f), m_Projection, m_View, Matrix.Identity);

            // Apply "Rotate 3D Around 2D Pivot" to world matrix.
            // Final Result = -Pivot[XYZ] * Rotation * Pivot[XYZ] * Translation
            m_World = Matrix.CreateTranslation(-vUnprojectPivot) * matRotation3D * Matrix.CreateTranslation(vUnprojectPivot) * Matrix.CreateTranslation(vTranslation);
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Calculate fill in shape by using triangulation.
        /// </summary>
        /// <param name="_WindingOrder">Winding order of the shape used for triangulating.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CalculateFillInShape(Triangulator.WindingOrder _WindingOrder)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count <= 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create a list of vertices.
            Vector2[] vSourceVertices = new Vector2[m_VectorList.Count];
            for (int i = m_VectorList.Count - 1; i >= 0; --i)
                vSourceVertices[i] = m_VectorList[i];
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create a list of indices.
            int[] nSourceIndices;

            // Triangulate vertices and indices.
            Triangulator.Triangulator.Triangulate(vSourceVertices,
                                                  _WindingOrder,
                                                  out vSourceVertices,
                                                  out nSourceIndices);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Store number of vertices and primitives for rendering.
            m_nVertices = vSourceVertices.Length;
            m_nPrimitives = nSourceIndices.Length / 3;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create and setup the list of vertex data.
            VertexPositionColor[] verts = new VertexPositionColor[vSourceVertices.Length];
            for (int i = vSourceVertices.Length - 1; i >= 0; --i)
                verts[i] = new VertexPositionColor(m_GraphicsDevice.Viewport.Unproject(new Vector3(vSourceVertices[i], 0f),
                                                                                                   m_Projection,
                                                                                                   m_View,
                                                                                                   Matrix.Identity), m_FillInColor);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Clean up old data.
            if (m_VertexBuffer != null)
            {
                m_VertexBuffer.Dispose();
                m_VertexBuffer = null;
            }
            if (m_IndexBuffer != null)
            {
                m_IndexBuffer.Dispose();
                m_IndexBuffer = null;
            }
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create vertex buffer.
            m_VertexBuffer = new VertexBuffer(m_GraphicsDevice,
                                              VertexPositionColor.VertexDeclaration,
                                              verts.Length * VertexPositionColor.VertexDeclaration.VertexStride,
                                              BufferUsage.WriteOnly);
            m_VertexBuffer.SetData(verts);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Branch here to convert our indices to shorts if possible for wider GPU support.
            if (verts.Length < UInt16.MaxValue /*65535*/)
            {
                //////////////////////////////////////////////////////////////////////////
                // Create a list of indices.
                short[] sIndices = new short[nSourceIndices.Length];
                for (int i = nSourceIndices.Length - 1; i >= 0; --i)
                    sIndices[i] = (short)nSourceIndices[i];
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Create index buffer.
                m_IndexBuffer = new IndexBuffer(m_GraphicsDevice,
                                                IndexElementSize.SixteenBits,
                                                sIndices.Length * sizeof(short),
                                                BufferUsage.WriteOnly);
                m_IndexBuffer.SetData(sIndices);
                //
                //////////////////////////////////////////////////////////////////////////
            }
            else
            {
                //////////////////////////////////////////////////////////////////////////
                // Create index buffer.
                m_IndexBuffer = new IndexBuffer(m_GraphicsDevice,
                                                IndexElementSize.ThirtyTwoBits,
                                                nSourceIndices.Length * sizeof(int),
                                                BufferUsage.WriteOnly);
                m_IndexBuffer.SetData(nSourceIndices);
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Calculate fill in shape by using triangulation.
        /// </summary>
        /// <param name="_WindingOrder">Winding order of the shape used for triangulating.</param>
        /// <param name="_vHoleList">List of vertices to cut out. The positions should be within the shape.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CalculateFillInShape(Triangulator.WindingOrder _WindingOrder, Vector2[] _vHoleList)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count <= 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create a list of vertices.
            Vector2[] vSourceVertices = new Vector2[m_VectorList.Count];
            for (int i = m_VectorList.Count - 1; i >= 0; --i)
                vSourceVertices[i] = m_VectorList[i];
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Cut hole in the shape.
            vSourceVertices = Triangulator.Triangulator.CutHoleInShape(vSourceVertices, _vHoleList);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create a list of indices.
            int[] nSourceIndices;

            // Triangulate vertices and indices.
            Triangulator.Triangulator.Triangulate(vSourceVertices,
                                                  _WindingOrder,
                                                  out vSourceVertices,
                                                  out nSourceIndices);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Store number of vertices and primitives for rendering.
            m_nVertices = vSourceVertices.Length;
            m_nPrimitives = nSourceIndices.Length / 3;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create and setup the list of vertex data.
            VertexPositionColor[] verts = new VertexPositionColor[vSourceVertices.Length];
            for (int i = vSourceVertices.Length - 1; i >= 0; --i)
                verts[i] = new VertexPositionColor(m_GraphicsDevice.Viewport.Unproject(new Vector3(vSourceVertices[i], 0f),
                                                                                                   m_Projection,
                                                                                                   m_View,
                                                                                                   Matrix.Identity), m_FillInColor);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create vertex buffer.
            m_VertexBuffer = new VertexBuffer(m_GraphicsDevice,
                                              VertexPositionColor.VertexDeclaration,
                                              verts.Length * VertexPositionColor.VertexDeclaration.VertexStride,
                                              BufferUsage.WriteOnly);
            m_VertexBuffer.SetData(verts);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Branch here to convert our indices to shorts if possible for wider GPU support.
            if (verts.Length < UInt16.MaxValue /*65535*/)
            {
                //////////////////////////////////////////////////////////////////////////
                // Create a list of indices.
                short[] sIndices = new short[nSourceIndices.Length];
                for (int i = nSourceIndices.Length - 1; i >= 0; --i)
                    sIndices[i] = (short)nSourceIndices[i];
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Create index buffer.
                m_IndexBuffer = new IndexBuffer(m_GraphicsDevice,
                                                IndexElementSize.SixteenBits,
                                                sIndices.Length * sizeof(short),
                                                BufferUsage.WriteOnly);
                m_IndexBuffer.SetData(sIndices);
                //
                //////////////////////////////////////////////////////////////////////////
            }
            else
            {
                //////////////////////////////////////////////////////////////////////////
                // Create index buffer.
                m_IndexBuffer = new IndexBuffer(m_GraphicsDevice,
                                                IndexElementSize.ThirtyTwoBits,
                                                nSourceIndices.Length * sizeof(int),
                                                BufferUsage.WriteOnly);
                m_IndexBuffer.SetData(nSourceIndices);
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Calculate fill in shape by using triangulation.
        /// </summary>
        /// <param name="_WindingOrder">Winding order of the shape used for triangulating.</param>
        /// <param name="_vHoleList">Multiple list of vertices to cut out. The positions should be within the shape.</param>
        //////////////////////////////////////////////////////////////////////////
        public void CalculateFillInShape(Triangulator.WindingOrder _WindingOrder, List<Vector2[]> _vHoleList)
        {
            //////////////////////////////////////////////////////////////////////////
            // Validate.
            if (m_VectorList.Count <= 2)
                return;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create a list of vertices.
            Vector2[] vSourceVertices = new Vector2[m_VectorList.Count];
            for (int i = m_VectorList.Count - 1; i >= 0; --i)
                vSourceVertices[i] = m_VectorList[i];
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Cut hole in the shape.
            foreach (Vector2[] vList in _vHoleList)
                vSourceVertices = Triangulator.Triangulator.CutHoleInShape(vSourceVertices, vList);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create a list of indices.
            int[] nSourceIndices;

            // Triangulate vertices and indices.
            Triangulator.Triangulator.Triangulate(vSourceVertices,
                                                  _WindingOrder,
                                                  out vSourceVertices,
                                                  out nSourceIndices);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Store number of vertices and primitives for rendering.
            m_nVertices = vSourceVertices.Length;
            m_nPrimitives = nSourceIndices.Length / 3;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create and setup the list of vertex data.
            VertexPositionColor[] verts = new VertexPositionColor[vSourceVertices.Length];
            for (int i = vSourceVertices.Length - 1; i >= 0; --i)
                verts[i] = new VertexPositionColor(m_GraphicsDevice.Viewport.Unproject(new Vector3(vSourceVertices[i], 0f),
                                                                                                   m_Projection,
                                                                                                   m_View,
                                                                                                   Matrix.Identity), m_FillInColor);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Create vertex buffer.
            m_VertexBuffer = new VertexBuffer(m_GraphicsDevice,
                                              VertexPositionColor.VertexDeclaration,
                                              verts.Length * VertexPositionColor.VertexDeclaration.VertexStride,
                                              BufferUsage.WriteOnly);
            m_VertexBuffer.SetData(verts);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Branch here to convert our indices to shorts if possible for wider GPU support.
            if (verts.Length < UInt16.MaxValue /*65535*/)
            {
                //////////////////////////////////////////////////////////////////////////
                // Create a list of indices.
                short[] sIndices = new short[nSourceIndices.Length];
                for (int i = nSourceIndices.Length - 1; i >= 0; --i)
                    sIndices[i] = (short)nSourceIndices[i];
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Create index buffer.
                m_IndexBuffer = new IndexBuffer(m_GraphicsDevice,
                                                IndexElementSize.SixteenBits,
                                                sIndices.Length * sizeof(short),
                                                BufferUsage.WriteOnly);
                m_IndexBuffer.SetData(sIndices);
                //
                //////////////////////////////////////////////////////////////////////////
            }
            else
            {
                //////////////////////////////////////////////////////////////////////////
                // Create index buffer.
                m_IndexBuffer = new IndexBuffer(m_GraphicsDevice,
                                                IndexElementSize.ThirtyTwoBits,
                                                nSourceIndices.Length * sizeof(int),
                                                BufferUsage.WriteOnly);
                m_IndexBuffer.SetData(nSourceIndices);
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////
        }

        #endregion // Public Methods

        #region Private Methods

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Calculate center position from the list of positions.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        private Vector2 CalculateCentroid()
        {
            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            float fArea = 0.0f, fDistance = 0.0f;
            Vector2 vCenter = Vector2.Zero;
            int nIndex = 0, nLastPointIndex = m_VectorList.Count - 1;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the list of positions.
            for (int i = 0; i <= nLastPointIndex; ++i)
            {
                //////////////////////////////////////////////////////////////////////////
                // Cacluate index.
                nIndex = (i + 1) % (nLastPointIndex + 1);

                // Calculate distance.
                fDistance = m_VectorList[i].X * m_VectorList[nIndex].Y - m_VectorList[nIndex].X * m_VectorList[i].Y;

                // Acculmate area.
                fArea += fDistance;

                // Move center positions based on positions and distance.
                vCenter.X += (m_VectorList[i].X + m_VectorList[nIndex].X) * fDistance;
                vCenter.Y += (m_VectorList[i].Y + m_VectorList[nIndex].Y) * fDistance;
            }
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Calculate the final center position.
            fArea *= 0.5f;
            vCenter.X *= 1.0f / (6.0f * fArea);
            vCenter.Y *= 1.0f / (6.0f * fArea);
            //
            //////////////////////////////////////////////////////////////////////////

            return vCenter;
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Calculate a spline based on 4 point positions and type.
        /// </summary>
        /// <param name="_type">Spline type to create.</param>
        /// <param name="_vPointList">List of points.</param>
        /// <param name="_fTension">Tension allows to tighten up the curvature. 1 (or greater) = high tension, 0 = normal tension, -1 (or lower) = low tension.</param>
        /// <param name="_fBias">Bias allows to twist the curve. 1 (or greater) = Towards first segment, 0 = even, -1 (or lower) = Towards end segment.</param>
        //////////////////////////////////////////////////////////////////////////
        private void CalculateSpline(Spline _type, Vector2[] _vPointList,
                                     float _fTension, float _fBias)
        {
            //////////////////////////////////////////////////////////////////////////
            // Clear out the list.
            m_VectorList.Clear();
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            float fTime = 0f, fTimeCube = 0f, fTimeSq = 0f;
            int nIndex = 0, nCurrentSegment = 0;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Calculate the number of segments, within the spline, using the end points.
            int nSegments = (int)(Math.Sqrt(Math.Pow((_vPointList[nIndex + 1].X - _vPointList[nIndex].X), 2) + Math.Pow((_vPointList[nIndex + 1].Y - _vPointList[nIndex].Y), 2)));
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the number of segments in reverse order.
            for (int i = nSegments; nCurrentSegment != _vPointList.Length - 1; --i)
            {
                //////////////////////////////////////////////////////////////////////////
                // Determine to add the next segment.
                if (i < 0)
                {
                    //////////////////////////////////////////////////////////////////////////
                    // Determine if we reached the end of the point list.
                    if (++nIndex >= _vPointList.Length - 1)
                        break;

                    // Otherwise continue on with the next segment on the spline.
                    ++nCurrentSegment;
                    i = nSegments = (int)(Math.Sqrt(Math.Pow((_vPointList[nIndex + 1].X - _vPointList[nIndex].X), 2) + Math.Pow((_vPointList[nIndex + 1].Y - _vPointList[nIndex].Y), 2)));
                    //
                    //////////////////////////////////////////////////////////////////////////
                }
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Determine the elapsed time value based on the segment between 0 and 1.
                fTime = (float)i / (float)nSegments;
                fTimeSq = fTime * fTime;
                fTimeCube = fTime * fTimeSq;
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Add the new position to the list.
                switch (nCurrentSegment)
                {
                    case 0: // End segment.
                        {
                            switch (_type)
                            {
                                case Spline.Cubic:
                                    {
                                        m_VectorList.Add(new Vector2(CubicInterpolate(_vPointList[1].X, _vPointList[2].X, _vPointList[3].X, GetPointOnSpline(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime).X, fTime),
                                                                     CubicInterpolate(_vPointList[1].Y, _vPointList[2].Y, _vPointList[3].Y, GetPointOnSpline(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime).Y, fTime)));
                                    }
                                    break;
                                case Spline.Linear:
                                    {
                                        m_VectorList.Add(new Vector2(LinearInterpolate(_vPointList[2].X, _vPointList[3].X, fTime),
                                                                     LinearInterpolate(_vPointList[2].Y, _vPointList[3].Y, fTime)));
                                    }
                                    break;
                                case Spline.Cosine:
                                    {
                                        m_VectorList.Add(Vector2.CatmullRom(_vPointList[1], _vPointList[2], _vPointList[3], GetPointOnSpline(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime), fTime));
                                    }
                                    break;
                                case Spline.Hermite:
                                    {
                                        m_VectorList.Add(new Vector2(HermiteInterpolate(_vPointList[1].X, _vPointList[2].X, _vPointList[3].X, GetPointOnSpline(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime).X, fTime, _fTension, _fBias),
                                                                     HermiteInterpolate(_vPointList[1].Y, _vPointList[2].Y, _vPointList[3].Y, GetPointOnSpline(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime).Y, fTime, _fTension, _fBias)));
                                    }
                                    break;

                                default: break;
                            }
                        }
                        break;
                    case 1: // Middle segment.
                        {
                            switch (_type)
                            {
                                case Spline.Cubic:
                                    {
                                        m_VectorList.Add(new Vector2(CubicInterpolate(_vPointList[0].X, _vPointList[1].X, _vPointList[2].X, _vPointList[3].X, fTime),
                                                                     CubicInterpolate(_vPointList[0].Y, _vPointList[1].Y, _vPointList[2].Y, _vPointList[3].Y, fTime)));
                                    }
                                    break;
                                case Spline.Linear:
                                    {
                                        m_VectorList.Add(new Vector2(LinearInterpolate(_vPointList[1].X, _vPointList[2].X, fTime),
                                                                     LinearInterpolate(_vPointList[1].Y, _vPointList[2].Y, fTime)));
                                    }
                                    break;
                                case Spline.Cosine:
                                    {
                                        m_VectorList.Add(Vector2.CatmullRom(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime));
                                    }
                                    break;
                                case Spline.Hermite:
                                    {
                                        m_VectorList.Add(new Vector2(HermiteInterpolate(_vPointList[0].X, _vPointList[1].X, _vPointList[2].X, _vPointList[3].X, fTime, _fTension, _fBias),
                                                                     HermiteInterpolate(_vPointList[0].Y, _vPointList[1].Y, _vPointList[2].Y, _vPointList[3].Y, fTime, _fTension, _fBias)));
                                    }
                                    break;

                                default: break;
                            }
                        }
                        break;
                    case 2: // First segment.
                        {
                            switch (_type)
                            {
                                case Spline.Cubic:
                                    {
                                        m_VectorList.Add(new Vector2(CubicInterpolate(GetPointOnSpline(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime).X, _vPointList[0].X, _vPointList[1].X, _vPointList[2].X, fTime),
                                                                     CubicInterpolate(GetPointOnSpline(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime).Y, _vPointList[0].Y, _vPointList[1].Y, _vPointList[2].Y, fTime)));
                                    }
                                    break;
                                case Spline.Linear:
                                    {
                                        m_VectorList.Add(new Vector2(LinearInterpolate(_vPointList[0].X, _vPointList[1].X, fTime),
                                                                     LinearInterpolate(_vPointList[0].Y, _vPointList[1].Y, fTime)));
                                    }
                                    break;
                                case Spline.Cosine:
                                    {
                                        m_VectorList.Add(Vector2.CatmullRom(GetPointOnSpline(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime), _vPointList[0], _vPointList[1], _vPointList[2], fTime));
                                    }
                                    break;
                                case Spline.Hermite:
                                    {
                                        m_VectorList.Add(new Vector2(HermiteInterpolate(GetPointOnSpline(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime).X, _vPointList[0].X, _vPointList[1].X, _vPointList[2].X, fTime, _fTension, _fBias),
                                                                     HermiteInterpolate(GetPointOnSpline(_vPointList[0], _vPointList[1], _vPointList[2], _vPointList[3], fTime).Y, _vPointList[0].Y, _vPointList[1].Y, _vPointList[2].Y, fTime, _fTension, _fBias)));
                                    }
                                    break;

                                default: break;
                            }
                        }
                        break;

                    default: break;
                }
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////

            // Calculate center average position.
            m_vCentriod = CalculateCentroid();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Calculate a bézier spline based on 4 point positions.
        /// </summary>
        /// <param name="_vPoint1">First position.</param>
        /// <param name="_vPoint2">Second position.</param>
        /// <param name="_vPoint3">Third position.</param>
        /// <param name="_vPoint4">Fourth position.</param>
        //////////////////////////////////////////////////////////////////////////
        private void BezierSpline(Vector2 _vPoint1, Vector2 _vPoint2, Vector2 _vPoint3, Vector2 _vPoint4)
        {
            //////////////////////////////////////////////////////////////////////////
            // Calculate the X values.
            float fX1 = _vPoint4.X - 3 * _vPoint3.X + 3 * _vPoint2.X - _vPoint1.X;
            float fX2 = 3 * _vPoint3.X - 6 * _vPoint2.X + 3 * _vPoint1.X;
            float fX3 = 3 * _vPoint2.X - 3 * _vPoint1.X;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Calculate the Y values.
            float fY1 = _vPoint4.Y - 3 * _vPoint3.Y + 3 * _vPoint2.Y - _vPoint1.Y;
            float fY2 = 3 * _vPoint3.Y - 6 * _vPoint2.Y + 3 * _vPoint1.Y;
            float fY3 = 3 * _vPoint2.Y - 3 * _vPoint1.Y;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Calculate the number of segments, within the spline, using the mid-points.
            int nSegments = (int)(Math.Sqrt(Math.Pow((_vPoint3.X - _vPoint2.X), 2) + Math.Pow((_vPoint3.Y - _vPoint2.Y), 2)));

            // Create a new list.
            m_VectorList = new List<Vector2>(nSegments);
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Local variables.
            float fTime = 0f, fTimeCube = 0f, fTimeSq = 0f;
            //
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Run through the number of segments in reverse order.
            for (int i = nSegments; i >= 0; --i)
            {
                //////////////////////////////////////////////////////////////////////////
                // Determine the elapsed time value based on the segment between 0 and 1.
                fTime = (float)i / (float)nSegments;
                fTimeSq = fTime * fTime;
                fTimeCube = fTime * fTimeSq;
                //
                //////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////
                // Add the new position to the list.
                m_VectorList.Add(new Vector2(fX1 * fTimeCube + fX2 * fTimeSq + fX3 * fTime + _vPoint1.X,
                                             fY1 * fTimeCube + fY2 * fTimeSq + fY3 * fTime + _vPoint1.Y));
                //
                //////////////////////////////////////////////////////////////////////////
            }
            //
            //////////////////////////////////////////////////////////////////////////

            // Calculate center average position.
            m_vCentriod = CalculateCentroid();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Get a point on the spline.
        /// </summary>
        /// <param name="_vP1">First position.</param>
        /// <param name="_vP2">Second position.</param>
        /// <param name="_vP3">Third position.</param>
        /// <param name="_vP4">Fourth position.</param>
        /// <param name="_fTime">Time elapsed between 0 and 1.</param>
        //////////////////////////////////////////////////////////////////////////
        public Vector2 GetPointOnSpline(Vector2 _vP1, Vector2 _vP2, Vector2 _vP3, Vector2 _vP4, float _fTime)
        {
            return (((_vP4 * _fTime) + _vP3) * _fTime + _vP2) * _fTime + _vP1;
        }

        #endregion // Private Methods

        #region Spline Interpolations

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Spline interpolation resulting in straight lines.
        /// </summary>
        /// <param name="_fP1">First point.</param>
        /// <param name="_fP2">Second point.</param>
        /// <param name="_fTime">Time elapsed between 0 and 1.</param>
        //////////////////////////////////////////////////////////////////////////
        float LinearInterpolate(float _fP1, float _fP2, float _fTime)
        {
            return (_fP1 * (1 - _fTime) + _fP2 * _fTime);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Spline interpolation resulting in smooth curvy lines.
        /// </summary>
        /// <param name="_fP1">First point.</param>
        /// <param name="_fP2">Second point.</param>
        /// <param name="_fP3">Third point.</param>
        /// <param name="_fP4">Fourth point.</param>
        /// <param name="_fTime">Time elapsed between 0 and 1.</param>
        //////////////////////////////////////////////////////////////////////////
        float CubicInterpolate(float _fP1, float _fP2, float _fP3, float _fP4, float _fTime)
        {
            float fTimeSq = _fTime * _fTime;
            float fA1 = _fP4 - _fP3 - _fP1 + _fP2;
            float fA2 = _fP1 - _fP2 - fA1;
            float fA3 = _fP3 - _fP1;
            float fA4 = _fP2;

            return (fA1 * _fTime * fTimeSq + fA2 * fTimeSq + fA3 * _fTime + fA4);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Spline interpolation resulting in, customizable, smooth curvy lines.
        /// </summary>
        /// <param name="_fP1">First point.</param>
        /// <param name="_fP2">Second point.</param>
        /// <param name="_fP3">Third point.</param>
        /// <param name="_fP4">Fourth point.</param>
        /// <param name="_fTime">Time elapsed between 0 and 1.</param>
        /// <param name="_fTension">Tension allows to tighten up the curvature. 1 (or greater) = high tension, 0 = normal tension, -1 (or lower) = low tension.</param>
        /// <param name="_fBias">Bias allows to twist the curve. 1 (or greater) = Towards first segment, 0 = even, -1 (or lower) = Towards end segment.</param>
        //////////////////////////////////////////////////////////////////////////
        float HermiteInterpolate(float _fP1, float _fP2, float _fP3, float _fP4,
                                 float _fTime, float _fTension, float _fBias)
        {
            float fTimeSq = _fTime * _fTime;
            float fTimeCube = fTimeSq * _fTime;
            float fTensionBias1 = (_fP2 - _fP1) * (1 + _fBias) * (1 - _fTension) / 2f;
            fTensionBias1 += (_fP3 - _fP2) * (1 - _fBias) * (1 - _fTension) / 2f;
            float fTensionBias2 = (_fP3 - _fP2) * (1 + _fBias) * (1 - _fTension) / 2f;
            fTensionBias2 += (_fP4 - _fP3) * (1 - _fBias) * (1 - _fTension) / 2f;
            float fA1 = 2 * fTimeCube - 3 * fTimeSq + 1;
            float fA2 = fTimeCube - 2 * fTimeSq + _fTime;
            float fA3 = fTimeCube - fTimeSq;
            float fA4 = -2 * fTimeCube + 3 * fTimeSq;

            return (fA1 * _fP2 + fA2 * fTensionBias1 + fA3 * fTensionBias2 + fA4 * _fP3);
        }

        #endregion // Spline Interpolations
    }
}