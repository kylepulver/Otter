using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// Graphic that renders as a simple gradient between 4 points.
    /// </summary>
    public class Gradient : Graphic {

        #region Private Fields

        List<Color> colors = new List<Color>();
        List<Color> baseColors = new List<Color>();

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Gradient using 4 Colors for each corner.
        /// </summary>
        /// <param name="width">The width of the gradient.</param>
        /// <param name="height">The height of the gradient.</param>
        /// <param name="TopLeft">The Color of the top left corner.</param>
        /// <param name="TopRight">The Color of the top right corner.</param>
        /// <param name="BottomRight">The Color of the bottom right corner.</param>
        /// <param name="BottomLeft">The Color of the bottom left corner.</param>
        public Gradient(int width, int height, Color TopLeft, Color TopRight, Color BottomRight, Color BottomLeft) {
            baseColors.Add(TopLeft);
            baseColors.Add(TopRight);
            baseColors.Add(BottomRight);
            baseColors.Add(BottomLeft);

            colors.Add(TopLeft);
            colors.Add(TopRight);
            colors.Add(BottomRight);
            colors.Add(BottomLeft);

            Width = width;
            Height = height;
        }

        /// <summary>
        /// Create a new Gradient using another Gradient.
        /// </summary>
        /// <param name="copy">The source Gradient to copy.</param>
        public Gradient(Gradient copy) : this(copy.Width, copy.Height, copy.GetColor(ColorPosition.TopLeft), copy.GetColor(ColorPosition.TopRight), copy.GetColor(ColorPosition.BottomRight), copy.GetColor(ColorPosition.BottomLeft)) { }

        #endregion

        #region Private Methods

        protected override void UpdateDrawable() {
            base.UpdateDrawable();

            SFMLVertices.Clear();

            var finalColors = new List<Color>() {
                Color.None,
                Color.None,
                Color.None,
                Color.None};
            for (int i = 0; i < baseColors.Count; i++) {
                finalColors[i] = new Color(baseColors[i]);
                finalColors[i] *= Color;
                finalColors[i].A *= Alpha;
            }

            SFMLVertices.Append(new Vertex(new Vector2f(0, 0), finalColors[0].SFMLColor));
            SFMLVertices.Append(new Vertex(new Vector2f(Width, 0), finalColors[1].SFMLColor));
            SFMLVertices.Append(new Vertex(new Vector2f(Width, Height), finalColors[2].SFMLColor));
            SFMLVertices.Append(new Vertex(new Vector2f(0, Height), finalColors[3].SFMLColor));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the Color of a specific position.
        /// </summary>
        /// <param name="color">The new Color.</param>
        /// <param name="position">The position to change the Color on.</param>
        public void SetColor(Color color, ColorPosition position) {
            colors[(int)position] = color;
            NeedsUpdate = true;
        }

        /// <summary>
        /// Get the Color of a specific position.
        /// </summary>
        /// <param name="position">The position to get the Color of.</param>
        /// <returns></returns>
        public Color GetColor(ColorPosition position) {
            return colors[(int)position];
        }

        #endregion

        #region Enum

        public enum ColorPosition {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft
        }

        #endregion
    }
}
