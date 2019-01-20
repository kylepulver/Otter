using SFML.Graphics;
using System;

namespace Otter {
    /// <summary>
    /// Graphic that is used to create a square image with a radial mask based on a value of 0 to 1.
    /// Something like the cool down timers on icons in various games.
    /// </summary>
    public class SquareClock : Graphic {

        #region Private Fields

        float fill = 1;

        #endregion

        #region Public Properties

        /// <summary>
        /// Determines the fill of the clock.
        /// </summary>
        public float Fill {
            set {
                fill = Util.Clamp(value, 0, 1);
                NeedsUpdate = true;
            }
            get {
                return fill;
            }
        }

        /// <summary>
        /// The current angle the clock is at.
        /// </summary>
        public float FillAngle {
            get { return (fill * 360) + 90; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new SquareClock.
        /// </summary>
        /// <param name="size">The width and height of the clock.</param>
        /// <param name="color">The fill Color.</param>
        public SquareClock(int size, Color color) {
            Width = size;
            Height = size;

            Color = color;
        }

        #endregion

        #region Private Methods

        protected override void UpdateDrawable() {
            base.UpdateDrawable();

            if (fill == 1) {
                //draw box
                SFMLVertices = new VertexArray(PrimitiveType.Quads);
                Append(SFMLVertices, 0, 0);
                Append(SFMLVertices, Width, 0);
                Append(SFMLVertices, Width, Height);
                Append(SFMLVertices, 0, Height);
            }
            else {

                SFMLVertices = new VertexArray(PrimitiveType.TrianglesFan);

                if (fill > 0) {
                    //draw center
                    Append(SFMLVertices, HalfWidth, HalfHeight);
                    //draw middle top
                    Append(SFMLVertices, HalfWidth, 0);
                    if (fill >= 0.125f) {
                        //draw left top
                        Append(SFMLVertices, 0, 0);
                    }
                    if (fill >= 0.375f) {
                        //draw left bottom
                        Append(SFMLVertices, 0, Height);
                    }
                    if (fill >= 0.625f) {
                        //draw right bottom
                        Append(SFMLVertices, Width, Height);
                    }
                    if (fill >= 0.875f) {
                        //draw right top
                        Append(SFMLVertices, Width, 0);
                    }

                    // get vector of angle
                    var v = new Vector2(Util.PolarX(FillAngle, HalfWidth), Util.PolarY(FillAngle, HalfHeight));
                    // adjust length of vector to meet square
                    var l = (float)Math.Max(Math.Abs(v.X), Math.Abs(v.Y));
                    if (l <= HalfWidth) {
                        v.X /= l;
                        v.Y /= l;
                    }
                    // append the vector
                    Append(SFMLVertices, HalfWidth + (float)v.X * HalfWidth, HalfHeight + (float)v.Y * HalfHeight);

                }
            }
        }

        void Append(VertexArray v, float x, float y) {
            v.Append(x, y, Color);
        }

        #endregion
 
    }
}
