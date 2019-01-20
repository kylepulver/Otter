using System;
namespace Otter {
    /// <summary>
    /// Point Collider.
    /// </summary>
    public class PointCollider : Collider {

        #region Constructors

        public PointCollider(int x, int y, params int[] tags) {
            Width = 1;
            Height = 1;
            X = x;
            Y = y;
            AddTag(tags);
            
        }

        public PointCollider(int x, int y, Enum tag, params Enum[] tags) : this(x, y) {
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

            Draw.Rectangle(Left, Top, 1, 1, color);
        }

        #endregion

    }
}
