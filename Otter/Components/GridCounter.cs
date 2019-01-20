using System;

namespace Otter {
    /// <summary>
    /// Counter in which the value can be moved in both an X and Y direction.  Probably most useful
    /// for making menus that are grids which the player can move around in.
    /// </summary>
    public class GridCounter : Component {

        #region Private Fields

        int x, y;

        #endregion

        #region Public Fields

        /// <summary>
        /// The width of the grid.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the grid.
        /// </summary>
        public int Height;

        /// <summary>
        /// Determines if the GridCounter should wrap horizontally.
        /// </summary>
        public bool WrapX;

        /// <summary>
        /// Determines if the GridCounter should wrap vertically.
        /// </summary>
        public bool WrapY;

        #endregion

        #region Public Properties

        /// <summary>
        /// The 1d value of the counter on the grid.
        /// </summary>
        public int Index {
            get { return Util.OneDee(Width, X, Y); }
            set {
                X = Util.TwoDeeX(value, Width);
                Y = Util.TwoDeeY(value, Width);
            }
        }

        /// <summary>
        /// Set both WrapX and WrapY.
        /// </summary>
        public bool Wrap {
            set { WrapX = value; WrapY = value; }
        }

        /// <summary>
        /// The total number of grid spaces.
        /// </summary>
        public int Count {
            get { return Width * Height; }
        }

        /// <summary>
        /// Move the index left.
        /// </summary>
        public void MoveLeft() {
            X -= 1;
        }

        /// <summary>
        /// Move the index right.
        /// </summary>
        public void MoveRight() {
            X += 1;
        }

        /// <summary>
        /// Move the index up.
        /// </summary>
        public void MoveUp() {
            Y -= 1;
        }

        /// <summary>
        /// Move the index down.
        /// </summary>
        public void MoveDown() {
            Y += 1;
        }

        /// <summary>
        /// The X value of the counter.
        /// </summary>
        public int X {
            set {
                if (WrapX) {
                    x = value;
                    while (x < 0) {
                        x += Width;
                    }
                    while (x > Width - 1) {
                        x -= Width;
                    }
                }
                else {
                    x = (int)Util.Clamp(value, Width - 1);
                }
            }
            get { return x; }
        }

        /// <summary>
        /// The Y value of the counter.
        /// </summary>
        public int Y {
            set {
                if (WrapY) {
                    y = value;
                    while (y < 0) {
                        y += Height;
                    }
                    while (y > Height - 1) {
                        y -= Height;
                    }
                }
                else {
                    y = (int)Util.Clamp(value, Height - 1);
                }
            }
            get { return y; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new GridCounter.
        /// </summary>
        /// <param name="value">The initial value of the GridCounter.</param>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        /// <param name="wrapX">Determines if the counter should wrap horizontally.</param>
        /// <param name="wrapY">Determines if the counter should wrap vertically.</param>
        public GridCounter(int value, int width = 1, int height = 1, bool wrapX = false, bool wrapY = false) {
            if (width < 1) {
                throw new ArgumentException("Width must be at least 1!");
            }
            if (height < 1) {
                throw new ArgumentException("Height must be at least 1!");
            }

            Width = width;
            Height = height;
            WrapX = wrapX;
            WrapY = wrapY;

            X = Util.TwoDeeX(value, width);
            Y = Util.TwoDeeY(value, width);
        }

        #endregion

        #region Operators

        public static implicit operator float(GridCounter counter) {
            return counter.Index;
        }
        public static implicit operator int(GridCounter counter) {
            return (int)counter.Index;
        }

        #endregion

    }
}
