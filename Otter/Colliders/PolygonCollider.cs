using System;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// Polygon Collider.  Only works with convex polygons!
    /// </summary>
    public class PolygonCollider : Collider {

        internal Polygon polygon;
        Vertices graphicVertices;

        public float Rotation;
        public float ScaleX = 1;
        public float ScaleY = 1;
        public bool FlippedX;
        public bool FlippedY;

        /// <summary>
        /// Determines if the Polygon will be returned transformed with the Rotation, Scale, and Flips.
        /// </summary>
        public bool AutoTransform = true;

        public PolygonCollider(Polygon polygon, params int[] tags) {
            this.polygon = polygon;
            AddTag(tags);
        }

        public PolygonCollider(Polygon polygon, Enum tag, params Enum[] tags) : this(polygon) {
            AddTag(tag);
            AddTag(tags);
        }

        public PolygonCollider(params float[] points) {
            var i = 0;
            float x = 0;
            List<Vector2> vectorPoints = new List<Vector2>();
            foreach (var p in points) {
                if (i == 0) {
                    x = p;
                    i = 1;
                }
                else {
                    vectorPoints.Add(new Vector2(x, p));
                    i = 0;
                }
            }
            if (i == 1) {
                vectorPoints.Add(new Vector2(x, 0));
            }

            this.polygon = new Polygon(points);
        }

        public List<Vector2> Points {
            get {
                return polygon.Points;
            }
        }

        public Vector2 this[int index] {
            get {
                return polygon[index];
            }
            set {
                polygon[index] = value;
            }
        }

        public int Count {
            get {
                return polygon.Count;
            }
        }

        public PolygonCollider(Vector2 firstPoint, params Vector2[] points) {
            polygon = new Polygon(firstPoint, points);
        }

        public override float Width {
            get {
                return polygon.Width;
            }
        }

        public override float Height {
            get {
                return polygon.Height;
            }
        }

        public Polygon Polygon {
            set {
                polygon = value;
            }
            get {
                if (!AutoTransform) return polygon;
                var transformed = new Polygon(polygon);
                transformed.Scale(ScaleX, ScaleY, OriginX, OriginY);
                transformed.Rotate(Rotation, OriginX, OriginY);
                transformed.Points.ForEach(v => {
                    if (FlippedY) {
                        v.Y -= OriginY;
                    }
                    if (FlippedX) {
                        v.X -= OriginX;
                    }
                });
                return transformed;
            }
        }

        public override void Render(Color color = null) {
            base.Render(color);
            if (color == null) color = Color.Red;

            if (Entity == null) return;

            graphicVertices = new Vertices();
            graphicVertices.PrimitiveType = VertexPrimitiveType.LinesStrip;
            foreach (var v in Polygon.Points) {
                graphicVertices.Add(new Vert(v.X, v.Y, color));
            }
            graphicVertices.Add(new Vert(Polygon.Points[0].X, Polygon.Points[0].Y, color));
            Draw.Graphic(graphicVertices, Left, Top);
        }
    }
}
