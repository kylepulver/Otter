using System;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// Class representing a Polygon.
    /// </summary>
    public class Polygon {

        #region Public Static Methods
   
        /// <summary>
        /// Creates a Polygon in the shape of a circle.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="steps">How many steps to use to create the circle (higher is rounder.)</param>
        /// <returns>A circle shaped Polygon.</returns>
        public static Polygon CreateCircle(float radius, int steps = 32) {
            var poly = new Polygon();
            (steps).Times((i) => {
                var angle = 360f / steps * i;
                poly.Add(Util.PolarX(angle, radius) + radius, Util.PolarY(angle, radius) + radius);
            });
            return poly;
        }

        /// <summary>
        /// Creates a Polygon in the shape of a rectangle.
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <returns>A rectangle shaped Polygon.</returns>
        public static Polygon CreateRectangle(float width, float height) {
            var poly = new Polygon();
            poly.Add(0, 0);
            poly.Add(width, 0);
            poly.Add(width, height);
            poly.Add(0, height);
            return poly;
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Create a new Polygon.
        /// </summary>
        /// <param name="points">The Vector2 points composing the Polygon.</param>
        public Polygon(Vector2 firstPoint, params Vector2[] points) {
            Points = new List<Vector2>();
            Points.Add(firstPoint);
            Points.AddRange(points);
        }

        /// <summary>
        /// Create a new Polygon.
        /// </summary>
        /// <param name="copy">The source Polygon to copy.</param>
        public Polygon(Polygon copy) {
            Points = new List<Vector2>();
            Points.AddRange(copy.Points);
        }

        /// <summary>
        /// Create a new Polygon.
        /// </summary>
        /// <param name="points">A series of points to create the Polygon from (x1, y1, x2, y2, x3, y3...)</param>
        public Polygon(params float[] points) {
            Points = new List<Vector2>();
            int i = 0;
            float x = 0;
            foreach (var p in points) {
                if (i == 0) {
                    x = p;
                    i = 1;
                }
                else {
                    Add(x, p);
                    i = 0;
                }
            }
            if (i == 1) {
                Add(x, 0);
            }
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The list of Vector2 points.
        /// </summary>
        public List<Vector2> Points { get; private set; }

        /// <summary>
        /// The number of points in the Polygon.
        /// </summary>
        public int Count {
            get {
                return Points.Count;
            }
        }

        /// <summary>
        /// The Width of the polygon determined by the right most point minus the left most point.
        /// </summary>
        public float Width {
            get {
                float min = float.MaxValue;
                float max = float.MinValue;
                foreach (var p in Points) {
                    min = Util.Min(min, p.X);
                    max = Util.Max(max, p.X);
                }
                return Math.Abs(max - min);
            }
        }

        /// <summary>
        /// The Height of the polygon determined by the bottom most point minus the top most point.
        /// </summary>
        public float Height {
            get {
                float min = float.MaxValue;
                float max = float.MinValue;
                foreach (var p in Points) {
                    min = Util.Min(min, p.Y);
                    max = Util.Max(max, p.Y);
                }
                return Math.Abs(max - min);
            }
        }

        /// <summary>
        /// Half of the Width.
        /// </summary>
        public float HalfWidth {
            get { return Width / 2f; }
        }

        /// <summary>
        /// Half of the Height.
        /// </summary>
        public float HalfHeight {
            get { return Height / 2f; }
        }

        public float Left {
            get {
                float min = Points[0].X;
                foreach (var p in Points) {
                    if (p.X > min) continue;
                    min = p.X;
                }
                return min;
            }
        }

        public float Right {
            get {
                float max = Points[0].X;
                foreach (var p in Points) {
                    if (p.X < max) continue;
                    max = p.X;
                }
                return max;
            }
        }

        public float Top {
            get {
                float min = Points[0].Y;
                foreach (var p in Points) {
                    if (p.Y > min) continue;
                    min = p.Y;
                }
                return min;
            }
        }

        public float Bottom {
            get {
                float max = Points[0].Y;
                foreach (var p in Points) {
                    if (p.Y < max) continue;
                    max = p.Y;
                }
                return max;
            }
        }

        #endregion Public Properties

        #region Public Indexers

        /// <summary>
        /// The list of Vector2 points in the Polygon.
        /// </summary>
        /// <param name="index">The index of the point.</param>
        /// <returns>The point at the specified index.</returns>
        public Vector2 this[int index] {
            get {
                return Points[index];
            }
            set {
                Points[index] = value;
            }
        }

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// Get a list of all the edges of the Polygon as Line2 objects.
        /// </summary>
        /// <returns>A Line2 list of all edges.</returns>
        public List<Line2> GetEdgesAsLines() {
            var list = new List<Line2>();

            for (var i = 0; i < Points.Count; i++) {
                Vector2 p1 = Points[i];
                Vector2 p2 = Points[i + 1 == Points.Count ? 0 : i + 1]; // Clever!
                var line = new Line2(p1, p2);
                list.Add(line);
            }

            return list;
        }
        /// <summary>
        /// Convert to a string.
        /// </summary>
        /// <returns>String of data about the Polygon.</returns>
        public override string ToString() {
            var str = "Polygon ";
            foreach (var p in Points) {
                str += string.Format("{0} ", p);
            }
            return str;
        }

        /// <summary>
        /// Offset all the points by a Vector2 amount.
        /// </summary>
        /// <param name="vector">The offset amount.</param>
        public void OffsetPoints(Vector2 vector) {
            for (int i = 0; i < Count; i++) {
                Points[i] += vector;
            }
        }

        /// <summary>
        /// Rotate the polygon by a specified amount.
        /// </summary>
        /// <param name="amount">The amount in degrees to rotate.</param>
        /// <param name="aroundX">The X position to rotate around.</param>
        /// <param name="aroundY">The Y position to rotate around.</param>
        public void Rotate(float amount, float aroundX, float aroundY) {
            for (int i = 0; i < Count; i++) {
                var p = Points[i];
                p = Util.RotateAround(p.X, p.Y, aroundX, aroundY, amount);
                Points[i] = p;
            }
        }

        /// <summary>
        /// Scale the polygon by a specified amount.
        /// </summary>
        /// <param name="amountX">The amount to scale horizontally.</param>
        /// <param name="amountY">The amount to scale veritcally.</param>
        /// <param name="aroundX">The X position to scale around.</param>
        /// <param name="aroundY">The Y position to scale around.</param>
        public void Scale(float amountX, float amountY, float aroundX, float aroundY) {
            for (int i = 0; i < Count; i++) {
                var p = Points[i];
                p.X -= aroundX;
                p.Y -= aroundY;

                p.X *= amountX;
                p.Y *= amountY;

                p.X += aroundX;
                p.Y += aroundY;

                Points[i] = p;
            }
        }

        /// <summary>
        /// Clear all points.
        /// </summary>
        public void Clear() {
            Points.Clear();
        }

        /// <summary>
        /// Offset all the points by a Vector2 amount.
        /// </summary>
        /// <param name="vector">The offset amount.</param>
        public void OffsetPoints(float x, float y) {
            OffsetPoints(new Vector2(x, y));
        }

        /// <summary>
        /// Check to see if a Polygon contains a point.
        /// </summary>
        /// <param name="point">The point to check for.</param>
        /// <returns>True if the polygon contains the point.</returns>
        public bool ContainsPoint(Vector2 point) {
            // I have no idea how this works http://stackoverflow.com/questions/11716268/point-in-polygon-algorithm
            int i, j, nvert = Points.Count;
            bool c = false;

            for(i = 0, j = nvert - 1; i < nvert; j = i++) {
                if(((Points[i].Y) >= point.Y ) != (Points[j].Y >= point.Y) && (point.X <= (Points[j].X - Points[i].X) * (point.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) + Points[i].X))
                    c = !c;
            }

            return c;
        }

        /// <summary>
        /// Check to see if a Polygon contains a point.
        /// </summary>
        /// <param name="x">The X position of the point to check for.</param>
        /// <param name="y">The Y position of the point to check for.</param>
        /// <returns>True if the polygon contains the point.</returns>
        public bool ContainsPoint(float x, float y) {
            return ContainsPoint(new Vector2(x, y));
        }

        /// <summary>
        /// Add a Vector2 to the list of points.
        /// </summary>
        /// <param name="point">The Vector2 to add the points.</param>
        public void Add(Vector2 point) {
            Points.Add(point);
        }

        /// <summary>
        /// Add an X Y position to the list of points.
        /// </summary>
        /// <param name="x">The X position to add.</param>
        /// <param name="y">The Y position to add.</param>
        public void Add(float x, float y) {
            Points.Add(new Vector2(x, y));
        }

        /// <summary>
        /// Project the polygon onto an axis.
        /// </summary>
        /// <param name="axis">The axis to project on.</param>
        /// <returns>The min and max values of the projection.</returns>
        public Range Projection(Vector2 axis) {
            if (Points.Count < 0) return new Range(0);

            float min = Vector2.Dot(axis, Points[0]);
            float max = min;

            for (var i = 0; i < Points.Count; i++) {
                float p = Vector2.Dot(axis, Points[i]);
                if (p < min) {
                    min = p;
                }
                else if (p > max) {
                    max = p;
                }
            }

            return new Range(min, max);
        }

        /// <summary>
        /// Get the axes to project on.
        /// </summary>
        /// <returns>A list of normals from the polygon.</returns>
        public List<Vector2> GetAxes() {
            var axes = new List<Vector2>();
            for (var i = 0; i < Points.Count; i++) {
                Vector2 p1 = Points[i];
                Vector2 p2 = Points[i + 1 == Points.Count ? 0 : i + 1]; // Clever!
                Vector2 edge = p1 - p2;
                Vector2 normal = new Vector2(-edge.Y, edge.X);
                axes.Add(normal);
            }
            return axes;
        }

        /// <summary>
        /// Test another Polygon for an overlap.  Will not work if either Polygon is concave!
        /// </summary>
        /// <param name="other">The other polygon to check.</param>
        /// <returns>True if this polygon overlaps the other polygon.</returns>
        public bool Overlap(Polygon other) {
            var axes = GetAxes();
            axes.AddRange(other.GetAxes());

            int i = 0;
            foreach (var axis in axes) {
                var p1 = Projection(axis);
                var p2 = other.Projection(axis);
                if (!p1.Overlap(p2)) {
                    return false;
                }
                i++;
            }

            return true;
        }

        #endregion Public Methods


    }
}
