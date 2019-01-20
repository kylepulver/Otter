using System.Collections.Generic;

namespace Otter {
    public class Bone : Component {

        public Bone Parent;
        public Skeleton Skeleton;
        public string Name;
        public int Depth { get; private set; }

        public List<Bone> Children = new List<Bone>();

        public bool AutoAddEntities;

        public bool InheritScale = false;
        public bool InheritRotation = true;

        public float X { get { return x; } set { LocalX = value; } }
        public float Y { get { return y; } set { LocalY = value; } }
        public float Rotation { get { return rotation; } set { LocalRotation = value; } }
        public float ScaleX { get { return scaleX; } set { LocalScaleX = value; } }
        public float ScaleY { get { return scaleY; } set { LocalScaleY = value; } }

        public bool FlipX { get { return flipX; } set { LocalFlipX = value; } }
        public bool FlipY { get { return flipY; } set { LocalFlipY = value; } }

        public float LocalX;
        public float LocalY;
        public float LocalRotation;
        public float LocalScaleX = 1;
        public float LocalScaleY = 1;

        public bool LocalFlipX;
        public bool LocalFlipY;

        public float BaseX { get; private set; }
        public float BaseY { get; private set; }
        public float BaseRotation { get; private set; }
        public float BaseScaleX { get; private set; }
        public float BaseScaleY { get; private set; }

        float x;
        float y;
        float rotation;
        float scaleX = 1;
        float scaleY = 1;
        bool flipX;
        bool flipY;

        float slotRotation;
        bool slotFlipX;
        bool slotFlipY;
        float slotScaleX;
        float slotScaleY;

        public bool FlipGraphicX;
        public bool FlipGraphicY;

        internal Entity BoneEntity { get; private set; }

        public struct BoneSlot {
            public float Rotation;
            public bool FlipX;
            public bool FlipY;
            public float ScaleX;
            public float ScaleY;
        }

        public BoneSlot Slot { get; private set; }

        public Bone(float x = 0, float y = 0, float rotation = 0, float scaleX = 1, float scaleY = 1) {
            BaseX = x;
            BaseY = y;
            BaseScaleX = scaleX;
            BaseScaleY = scaleY;
            BaseRotation = rotation;
            ScaleX = 1;
            ScaleY = 1;

            Visible = false;
        }

        public Bone(Entity e, float x = 0, float y = 0, float rotation = 0, float scaleX = 1, float scaleY = 1) : this(x, y, rotation, scaleX, scaleY) {
            SetEntity(e);
        }

        /// <summary>
        /// Add a bone as a child of this bone.  This should be done via a Skeleton!
        /// </summary>
        /// <param name="e">The bone to add.</param>
        /// <returns>The added bone.</returns>
        public Bone AddBone(Bone e) {
            Children.Add(e);
            e.Parent = this;
            return e;
        }

        /// <summary>
        /// Gets all the children of a specified Bone.
        /// </summary>
        /// <returns>A list of bones that are the children of the specified Bone.</returns>
        public List<Bone> GetAllChildren() {
            var bones = new List<Bone>();
            foreach (var c in Children) {
                bones.AddRange(c.GetAllChildren());
                bones.Add(c);
            }
            return bones;
        }

        public Bone SetEntity(Entity e) {
            e.AddComponent(this);
            BoneEntity = e;
            return this;
        }

        public override void Added() {
            base.Added();
            BoneEntity = null;
        }

        public void UpdateTransforms() {
            LocalRotation = Util.WrapAngle(LocalRotation); // Don't want the angle to get CRAZY

            if (Parent != null) {
                flipX = LocalFlipX ^ Parent.FlipX;
                flipY = LocalFlipY ^ Parent.FlipY;
                var pos = new Vector2(Parent.FlipX ? -LocalX - BaseX : LocalX + BaseX, Parent.FlipY ? -LocalY - BaseY : LocalY + BaseY);

                if (InheritScale) {
                    scaleX = Parent.ScaleX * LocalScaleX * BaseScaleX;
                    scaleY = Parent.ScaleY * LocalScaleY * BaseScaleY;
                }
                else {
                    scaleX = LocalScaleX * BaseScaleX;
                    scaleY = LocalScaleY * BaseScaleY;
                }

                pos.X *= Parent.ScaleX;
                pos.Y *= Parent.ScaleY;

                pos.X += Parent.X;
                pos.Y += Parent.Y;

                if (InheritRotation) {
                    rotation = Parent.Rotation + LocalRotation + BaseRotation;
                }
                else {
                    rotation = LocalRotation + BaseRotation;
                }

                pos = Util.RotateAround(pos.X, pos.Y, Parent.X, Parent.Y, (Parent.FlipX ^ Parent.FlipY ? -Parent.Rotation : Parent.Rotation));

                x = pos.X;
                y = pos.Y;

                Depth = Parent.Depth + 1;
            }
            else {
                rotation = LocalRotation + BaseRotation;
                scaleX = LocalScaleX * BaseScaleX;
                scaleY = LocalScaleY * BaseScaleY;
                x = LocalX + BaseX;
                y = LocalY + BaseY;
                flipX = LocalFlipX;
                flipY = LocalFlipY;

                Depth = 0;
            }

            rotation = Util.WrapAngle(rotation); // No crazy angles please.

            if (Entity != null) {
                var flipGraphicX = false;
                var flipGraphicY = false;

                if (FlipX && !FlipY) {
                    slotRotation = 180 - Rotation;
                    flipGraphicY = true;
                }
                else if (FlipY && !FlipX) {
                    slotRotation = -Rotation;
                    flipGraphicY = true;
                }
                else if (FlipX && FlipY) {
                    slotRotation = 180 + Rotation;
                }
                else {
                    slotRotation = Rotation;
                }

                if (InheritScale) {
                    slotScaleX = ScaleX;
                    slotScaleY = ScaleY;
                }
                else {
                    slotScaleX = LocalScaleX * BaseScaleX;
                    slotScaleY = LocalScaleY * BaseScaleY;
                }

                slotFlipX = flipGraphicX ^ FlipGraphicX;
                slotFlipY = flipGraphicY ^ FlipGraphicY;

                Entity.X = X;
                Entity.Y = Y;

                Slot = new BoneSlot() {
                    FlipX = slotFlipX,
                    FlipY = slotFlipY,
                    Rotation = slotRotation,
                    ScaleX = slotScaleX,
                    ScaleY = slotScaleY
                };

                if (!Entity.IsInScene && AutoAddEntities) {
                    if (Skeleton.Entity.IsInScene) {
                        Skeleton.Entity.Scene.Add(Entity);
                    }
                }
            }

            foreach (var c in Children) {
                c.UpdateTransforms();
            }
        }

        public override void Render() {
            base.Render();
            //Draw.Circle(X - 3, Y - 3, 3, Color.Black, Color.White, 3);
        }

    }
}
