using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Graphic that renders as a checkerboard type grid that fills the defined area using two alternating
    /// colors.
    /// </summary>
    public class Grid : Graphic {

        #region Public Fields

        /// <summary>
        /// The first Color to use for the Grid.
        /// </summary>
        public Color ColorA;

        /// <summary>
        /// The second Color to use for the Grid.
        /// </summary>
        public Color ColorB;

        /// <summary>
        /// The width of each cell on the Grid.
        /// </summary>
        public float GridWidth;

        /// <summary>
        /// The height of each cell on the Grid.
        /// </summary>
        public float GridHeight;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Grid.
        /// </summary>
        /// <param name="width">The width of the Grid in pixels.</param>
        /// <param name="height">The height of the Gird in pixels.</param>
        /// <param name="gridWidth">The width of each cell on the Grid.</param>
        /// <param name="gridHeight">The height of each cell on the Grid.</param>
        /// <param name="colorA">The first Color of the Grid.</param>
        /// <param name="colorB">The second Color of the Grid.</param>
        public Grid(int width, int height, int gridWidth, int gridHeight, Color colorA, Color colorB = null) {
            Width = width;
            Height = height;

            ColorA = colorA;

            if (colorB == null) {
                ColorB = colorA.Copy();
                ColorB.R -= 0.02f;
                ColorB.G -= 0.02f;
                ColorB.B -= 0.02f;
            }
            else {
                ColorB = colorB;
            }

            GridWidth = gridWidth;
            GridHeight = gridHeight;
        }

        #endregion

        #region Private Methods

        protected override void UpdateDrawable() {
            base.UpdateDrawable();

            Color nextColor = ColorA;
            Color rowColor = nextColor;
            SFMLVertices = new VertexArray(PrimitiveType.Quads);
            for (float j = 0; j < Height; j += GridHeight) {
                for (float i = 0; i < Width; i += GridWidth) {
                    var color = new Color(nextColor) * Color;
                    SFMLVertices.Append(new Vertex(new Vector2f(i, j), color.SFMLColor));
                    SFMLVertices.Append(new Vertex(new Vector2f(i + GridWidth, j), color.SFMLColor));
                    SFMLVertices.Append(new Vertex(new Vector2f(i + GridWidth, j + GridHeight), color.SFMLColor));
                    SFMLVertices.Append(new Vertex(new Vector2f(i, j + GridHeight), color.SFMLColor));
                    nextColor = nextColor == ColorA ? ColorB : ColorA;
                }
                rowColor = nextColor = rowColor == ColorA ? ColorB : ColorA;
            }
        }

        #endregion

    }
}
