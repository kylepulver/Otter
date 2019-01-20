namespace Otter {

    /// <summary>
    /// Class representing one piece of a Snake.
    /// </summary>
    public class Vertebra : Component {

        #region Public Fields

        /// <summary>
        /// The distance from the previous Vertabra in the Snake.
        /// </summary>
        public int Distance;

        /// <summary>
        /// The slot that contains the final transformation of the Vertebra.
        /// </summary>
        public VertebraSlot Slot;

        /// <summary>
        /// The Snake that this Vertebra belongs to.
        /// </summary>
        public Snake Snake;

        /// <summary>
        /// The total distance from the head of the Snake for this Vertebra.
        /// </summary>
        public int TotalDistance;

        /// <summary>
        /// Determines if the Vertebra will automatically add its Entity to the Scene.
        /// </summary>
        public bool AutoAddEntities;

        #endregion Public Fields

        #region Private Fields

        private float rotation;

        private float slotRotation;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// The local rotation of the Vertebra.
        /// </summary>
        public float LocalRotation { get; private set; }

        /// <summary>
        /// The rotation of the Vertebra.  When setting this the LocalRotation will be set.
        /// </summary>
        public float Rotation {
            get {
                return rotation + LocalRotation;
            }
            set {
                LocalRotation = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sets the Entity of the Vertebra (another way to add this component to an Entity.)
        /// </summary>
        /// <param name="e">The Entity to assign to this Vertebra.</param>
        public void SetEntity(Entity e) {
            e.AddComponent(this);
        }

        public override void Update() {
            Entity.SetPosition(Snake.GetPosition(TotalDistance));

            var lookFrom = Snake.GetPosition(TotalDistance + 1);
            rotation = Util.Angle(lookFrom.X, lookFrom.Y, Entity.X, Entity.Y);

            slotRotation = Rotation;

            Slot = new VertebraSlot() {
                Rotation = slotRotation
            };

            if (!Entity.IsInScene && AutoAddEntities) {
                if (Snake.Entity.IsInScene) {
                    Snake.Entity.Scene.Add(Entity);
                }
            }
        }

        #endregion Public Methods

        #region Public Structs

        /// <summary>
        /// A struct containing the final transformation of the Vertebra from the Snake.
        /// </summary>
        public struct VertebraSlot {
            /// <summary>
            /// The final transformed rotation of the Vertebra.
            /// </summary>
            public float Rotation;
        }

        #endregion Public Structs
    }
}