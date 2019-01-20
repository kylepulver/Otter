using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// Graphic used for rendering a set of Vert objects.  Basically a wrapper for SFML's VertexArray.
    /// </summary>
    public class Vertices : Graphic {

        #region Private Fields

        VertexPrimitiveType primitiveType = VertexPrimitiveType.Quads;

        #endregion

        #region Public Fields

        /// <summary>
        /// The list of Verts.
        /// </summary>
        public List<Vert> Verts = new List<Vert>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The primitive type drawing mode for the Verts.
        /// </summary>
        public VertexPrimitiveType PrimitiveType {
            get {
                return primitiveType;
            }
            set {
                primitiveType = value;
                NeedsUpdate = true;
            }
        }

        #endregion

        #region Static Fields

        public static VertexPrimitiveType DefaultPrimitiveType = VertexPrimitiveType.TrianglesFan;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new set of Vertices using a file path to a Texture.
        /// </summary>
        /// <param name="source">The file path to the Texture.</param>
        /// <param name="vertices">The Verts to use.</param>
        public Vertices(string source, params Vert[] vertices)
            : this(vertices) {
            SetTexture(new Texture(source));
            Initialize(vertices);
        }

        /// <summary>
        /// Create a new set of Vertices using a Texture.
        /// </summary>
        /// <param name="texture">The Texture to use.</param>
        /// <param name="vertices">The Verts to use.</param>
        public Vertices(Texture texture, params Vert[] vertices)
            : this(vertices) {
            SetTexture(texture);
            Initialize(vertices);
        }

        /// <summary>
        /// Create a new set of Vertices using an AtlasTexture.
        /// </summary>
        /// <param name="texture">The AtlasTexture to use.</param>
        /// <param name="vertices">The Verts to use.</param>
        public Vertices(AtlasTexture texture, params Vert[] vertices)
            : this(vertices) {
            SetTexture(texture);
            Initialize(vertices);
        }

        /// <summary>
        /// Create a new set of Vertices with no Texture.
        /// </summary>
        /// <param name="vertices">The Verts to use.</param>
        public Vertices(params Vert[] vertices)
            : base() {
            Initialize(vertices);
        }

        #endregion

        #region Private Methods

        void Initialize(params Vert[] vertices) {
            PrimitiveType = DefaultPrimitiveType;
            Add(vertices);
        }

        protected override void UpdateDrawable() {
            base.UpdateDrawable();

            SFMLVertices = new VertexArray((SFML.Graphics.PrimitiveType)PrimitiveType);

            
            foreach (var v in Verts) {
                // Adjust texture for potential atlas offset.
                v.U += TextureLeft;
                v.V += TextureTop;
                v.U = Util.Clamp(v.U, TextureLeft, TextureRight);
                v.V = Util.Clamp(v.V, TextureTop, TextureBottom);

                //copy to new vert and apply color and alpha
                var vCopy = new Vert(v);
                vCopy.Color *= Color;
                vCopy.Color.A *= Alpha;

                SFMLVertices.Append(vCopy);
            }

            
        }

        void UpdateDimensions() {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var v in Verts) {
                minX = Util.Min(v.X, minX);
                minY = Util.Min(v.Y, minY);

                maxX = Util.Max(v.X, maxX);
                maxY = Util.Max(v.Y, maxY);
            }

            Width = (int)Util.Round(Math.Abs(maxX - minX));
            Height = (int)Util.Round(Math.Abs(maxY - minY));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears all Verts from the Vertices.
        /// </summary>
        public void Clear() {
            Verts.Clear();
            NeedsUpdate = true;
            UpdateDimensions();
        }

        /// <summary>
        /// Add a Vert.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="color">The Color.</param>
        /// <param name="u">The X position on the Texture.</param>
        /// <param name="v">The Y position on the Texture.</param>
        public void Add(float x, float y, Color color, float u, float v) {
            var vert = new Vert(x, y, color, u, v);
            Add(vert);
        }

        /// <summary>
        /// Add a Vert.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="u">The X position on the Texture.</param>
        /// <param name="v">The Y position on the Texture.</param>
        public void Add(float x, float y, float u, float v) {
            var vert = new Vert(x, y, u, v);
            Add(vert);
        }

        /// <summary>
        /// Add a Vert.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="color">The Color.</param>
        public void Add(float x, float y, Color color) {
            var vert = new Vert(x, y, color);
            Add(vert);
        }

        /// <summary>
        /// Add a Vert.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        public void Add(float x, float y) {
            var vert = new Vert(x, y);
            Add(vert);
        }

        /// <summary>
        /// Add a set of Verts.
        /// </summary>
        /// <param name="vertices">The Verts to add.</param>
        public void Add(params Vert[] vertices) {
            foreach (var v in vertices) {
                Verts.Add(v);
            }
            NeedsUpdate = true;
            UpdateDimensions();
        }

        /// <summary>
        /// Remove Verts.
        /// </summary>
        /// <param name="vertices">The Verts to remove.</param>
        public void Remove(params Vert[] vertices) {
            foreach (var v in vertices) {
                Verts.RemoveIfContains(v);
            }
            NeedsUpdate = true;
            UpdateDimensions();
        }

        /// <summary>
        /// Remove Verts at a specific coordinate.
        /// </summary>
        /// <param name="x">The X position of the Vert to remove.</param>
        /// <param name="y">The Y position of the Vert to remove.</param>
        public void RemoveAt(float x, float y) {
            var vertsToRemove = new List<Vert>();
            foreach (var v in Verts) {
                if (v.X == x && v.Y == y) {
                    vertsToRemove.Add(v);
                }
            }
            Remove(vertsToRemove.ToArray());
        }

        #endregion

        #region Internal

        internal SFML.Graphics.VertexArray SFMLVertexArray {
            get {
                Update(); //update if needed
                return SFMLVertices;
            }
        }

        #endregion
       
    }

    #region Enum

    public enum VertexPrimitiveType {
        Points = 0,
        Lines = 1,
        LinesStrip = 2,
        Triangles = 3,
        TrianglesStrip = 4,
        TrianglesFan = 5,
        Quads = 6,
    }

    #endregion

}
