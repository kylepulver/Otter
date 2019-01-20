using System;

namespace Otter {
    /// <summary>
    /// Line Collider.
    /// </summary>
    public class LineCollider : Collider {

        #region Public Fields

        /// <summary>
        /// The X position of the end of the line.
        /// </summary>
        public float X2;

        /// <summary>
        /// The Y position of the end of the line.
        /// </summary>
        public float Y2;

        #endregion

        #region Public Properties

        /// <summary>
        /// The width of the area the line occupies.
        /// </summary>
        public override float Width {
            get { return Math.Abs(X - X2); }
        }

        /// <summary>
        /// The height of the area the line occupies.
        /// </summary>
        public override float Height {
            get { return Math.Abs(Y - Y2); }
        }

        /// <summary>
        /// The bottom most Y position of the line.
        /// </summary>
        public override float Bottom {
            get { return Math.Max(Y, Y2) - OriginY + Entity.Y; }
        }

        /// <summary>
        /// The top most Y position of the line.
        /// </summary>
        public override float Top {
            get { return Math.Min(Y, Y2) - OriginY + Entity.Y; }
        }

        /// <summary>
        /// The left most X position of the line.
        /// </summary>
        public override float Left {
            get { return Math.Min(X, X2) - OriginX + Entity.X; }
        }

        /// <summary>
        /// The right most X position of the line.
        /// </summary>
        public override float Right {
            get { return Math.Max(X, X2) - OriginX + Entity.X; }
        }

        /// <summary>
        /// Convert the LineCollider into a Line2 object.
        /// </summary>
        public Line2 Line2 {
            get { return new Line2(X - OriginX + Entity.X, Y - OriginY + Entity.Y, X2 - OriginX + Entity.X, Y2 - OriginY + Entity.Y); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a LineCollider.
        /// </summary>
        /// <param name="x1">The X position of the start of the line.</param>
        /// <param name="y1">The Y position of the start of the line.</param>
        /// <param name="x2">The X position of the end of the line.</param>
        /// <param name="y2">The X position of the end of the line.</param>
        /// <param name="tags">The tags to register for the Collider.</param>
        public LineCollider(float x1, float y1, float x2, float y2, params int[] tags) {
            X = x1;
            Y = y1;
            X2 = x2;
            Y2 = y2;

            AddTag(tags);
        }

        public LineCollider(float x1, float y1, float x2, float y2, Enum tag, params Enum[] tags) : this(x1, y1, x2, y2) {
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

            Draw.Line(Line2.X1, Line2.Y1, Line2.X2, Line2.Y2, color);
        }

        #endregion
        
    }
}
