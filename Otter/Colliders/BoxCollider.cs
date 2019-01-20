using System;
namespace Otter {
    /// <summary>
    /// Rectangle Collider.
    /// </summary>
    public class BoxCollider : Collider {

        #region Constructors

        /// <summary>
        /// Creates a new box collider.
        /// </summary>
        /// <param name="width">The width of the collider.</param>
        /// <param name="height">The height of the collider.</param>
        /// <param name="tags">Any tags the collider should have.</param>
        public BoxCollider(int width, int height, params int[] tags) {
            Width = width;
            Height = height;
            AddTag(tags);
        }

        public BoxCollider(int width, int height, Enum tag, params Enum[] tags) : this(width, height) {
            AddTag(tag);
            AddTag(tags);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Draw the collider for debug purposes.
        /// </summary>
        public override void Render(Color color = null) {
            base.Render(color);
            if (color == null) color = Color.Red;

            if (Entity == null) return;

            if (Width <= 2 || Height <= 2) {
                Draw.Rectangle(Left, Top, Width, Height, color);
            }
            else {
                Draw.Rectangle(Left + 1, Top + 1, Width - 2, Height - 2, Color.None, color, 1f);
            }
        }

        #endregion

    }
}
