using System;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// A base class for Movement Components to extend.
    /// </summary>
    public abstract class Movement : Component {

        #region Private Fields

        protected int
            MoveBufferX = 0,
            MoveBufferY = 0;

        #endregion

        #region Public Fields

        /// <summary>
        /// An action triggered after movement as been applied. Up to subclasses to implement.
        /// </summary>
        public Action OnMove;

        /// <summary>
        /// Determines how many units represent 1 pixel. Default is 100. Example: A speed of 100 would
        /// move the object 1 pixel per update.
        /// </summary>
        public int SpeedScale = 100;

        /// <summary>
        /// The main Collider to use for detecting collisions. If this is not set, no collisions will
        /// register at all!
        /// </summary>
        public new Collider Collider;

        /// <summary>
        /// An action triggered when there is a collision in the X axis.
        /// </summary>
        public Action OnCollideX;

        /// <summary>
        /// An action triggered when there is a collision in the Y axis.
        /// </summary>
        public Action OnCollideY;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of types that the Collider should check for collisions for when moving.
        /// </summary>
        public List<int> CollisionsSolid { get; private set; }


        #endregion

        #region Constructors

        public Movement() {
            CollisionsSolid = new List<int>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a tag that the Collider should check for collisions with.
        /// </summary>
        /// <param name="tags">The tags to collide with.</param>
        public void AddCollision(params int[] tags) {
            foreach (var t in tags) {
                CollisionsSolid.Add(t);
            }
        }

        /// <summary>
        /// Register a tag that the Collider should check for collisions with.
        /// </summary>
        /// <param name="tags">The tags to collide with.</param>
        public void AddCollision(params Enum[] tags) {
            AddCollision(Util.EnumToIntArray(tags));
        }

        /// <summary>
        /// Remove a tag from the collision checking.
        /// </summary>
        /// <param name="tags"></param>
        public void RemoveCollision(params int[] tags) {
            foreach (int t in tags) {
                if (CollisionsSolid.Contains(t)) {
                    CollisionsSolid.Remove(t);
                }
            }
        }

        /// <summary>
        /// Move the object in the X axis by the value of speed. Sweeping collision test.
        /// </summary>
        /// <param name="speed">The speed to move by (remember SpeedScale is applied.)</param>
        /// <param name="collider">The Collider to use when moving.</param>
        public virtual void MoveX(int speed, Collider collider = null) {
            MoveBufferX += speed;

            while (Math.Abs(MoveBufferX) >= SpeedScale) {
                int move = Math.Sign(MoveBufferX);
                if (collider != null) {
                    Collider c = collider.Collide(Entity.X + move, Entity.Y, CollisionsSolid);
                    if (c == null) {
                        Entity.X += move;
                        MoveBufferX = (int)Util.Approach(MoveBufferX, 0, SpeedScale);
                    }
                    else {
                        MoveBufferX = 0;
                        MoveCollideX(c);
                    }
                }
                if (collider == null || CollisionsSolid.Count == 0) {
                    Entity.X += move;
                    MoveBufferX = (int)Util.Approach(MoveBufferX, 0, SpeedScale);
                }

            }
        }

        /// <summary>
        /// Move the object in the Y axis by the value of speed. Sweeping collision test.
        /// </summary>
        /// <param name="speed">The speed to move by (remember SpeedScale is applied.)</param>
        /// <param name="collider">The Collider to use when moving.</param>
        public virtual void MoveY(int speed, Collider collider = null) {
            MoveBufferY += speed;

            while (Math.Abs(MoveBufferY) >= SpeedScale) {
                int move = Math.Sign(MoveBufferY);
                if (collider != null) {
                    Collider c = collider.Collide(Entity.X, Entity.Y + move, CollisionsSolid);
                    if (c == null) {
                        Entity.Y += move;
                        MoveBufferY = (int)Util.Approach(MoveBufferY, 0, SpeedScale);
                    }
                    else {
                        MoveBufferY = 0;
                        MoveCollideY(c);
                    }
                }
                if (collider == null || CollisionsSolid.Count == 0) {
                    Entity.Y += move;
                    MoveBufferY = (int)Util.Approach(MoveBufferY, 0, SpeedScale);
                }
            }
        }

        /// <summary>
        /// Called when MoveX collides with a collider.
        /// </summary>
        /// <param name="collider">The collider that was hit.</param>
        public virtual void MoveCollideX(Collider collider) {
            if (OnCollideX != null) {
                OnCollideX();
            }
        }

        /// <summary>
        /// Called when MoveY collides with a collider.
        /// </summary>
        /// <param name="collider">The collider that was hit.</param>
        public virtual void MoveCollideY(Collider collider) {
            if (OnCollideY != null) {
                OnCollideY();
            }
        }

        /// <summary>
        /// Shortcut to call both move x and move y.
        /// </summary>
        /// <param name="speedX"></param>
        /// <param name="speedY"></param>
        /// <param name="collider"></param>
        public void MoveXY(int speedX, int speedY, Collider collider = null) {
            MoveX(speedX, collider);
            MoveY(speedY, collider);
        }

        #endregion
    }
}
