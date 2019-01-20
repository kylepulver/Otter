using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Transformation {

        public Vector2 Translation;
        public Vector2 Scale;
        public Vector2 Origin;
        public float Rotation;

        public Transformation(Vector2 translation, Vector2 scale, Vector2 origin, float rotation) {
            Translation = translation;
            Scale = scale;
            Origin = origin;
            Rotation = rotation;
        }

        public Transformation() {
            Translation = Vector2.Zero;
            Scale = Vector2.One;
            Origin = Vector2.Zero;
            Rotation = 0;
        }

        public float ScaleX {
            get {
                return Scale.X;
            }
            set {
                Scale.X = value;
            }
        }

        public float ScaleY {
            get {
                return Scale.Y;
            }
            set {
                Scale.Y = value;
            }
        }

        public float X {
            get {
                return Translation.X;
            }
            set {
                Translation.X = value;
            }
        }

        public float Y {
            get {
                return Translation.Y;
            }
            set {
                Translation.Y = value;
            }
        }

        public float OriginX {
            get {
                return Origin.X;
            }
            set {
                Origin.X = value;
            }
        }

        public float OriginY {
            get {
                return Origin.Y;
            }
            set {
                Origin.Y = value;
            }
        }

        public float Angle {
            get {
                return Rotation;
            }
            set {
                Rotation = value;
            }
        }
    }
}
