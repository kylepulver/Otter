using System.Collections.Generic;
using System.Linq;

namespace Otter {
    public class Snake : Component {

        public List<Vertebra> Vertebrae = new List<Vertebra>();

        public float UpdateThreshold = 0.5f;

        List<Vector2> positionLog = new List<Vector2>();

        float lastX, lastY;

        public int MaxLength { get; private set; }

        public Vertebra Head {
            get {
                return Vertebrae.First();
            }
        }

        public void Add(Vertebra v) {
            Vertebrae.Add(v);
        }

        public void Add(Entity e, int distance) {
            var v = new Vertebra();
            v.SetEntity(e);
            v.Distance = distance;
            v.Snake = this;
            Add(v);
        }

        public Snake() {
            MaxLength = int.MaxValue;
        }

        public override void Added() {
            base.Added();
            positionLog.Insert(0, new Vector2(Entity.X, Entity.Y));
        }

        public override void Update() {
            base.Update();

            if (Util.Distance(lastX, lastY, Entity.X, Entity.Y) >= UpdateThreshold) { 
                positionLog.Insert(0, new Vector2(Entity.X, Entity.Y));
                if (positionLog.Count > MaxLength) {
                    positionLog.RemoveAt(MaxLength);
                }
            }

            var dist = 0;
            for (int i = 0; i < Vertebrae.Count; i++) {
                var v = Vertebrae[i];

                dist += v.Distance;
                v.TotalDistance = dist;

                v.Update();
            }

            MaxLength = dist + 2; // Add 2 so the last vertebra can rotate correctly.

            // Log the position.
            lastX = Entity.X;
            lastY = Entity.Y;
        }

        public Vector2 GetPosition(int distance) {
            if (distance >= positionLog.Count) {
                return positionLog.Last();
            }
            if (distance < 0) {
                return positionLog.First();
            }
            return positionLog[distance];
        }

        public void AddAllVertebraeToScene() {
            if (!Entity.IsInScene) return;
            foreach (var v in Vertebrae) {
                if (v.Entity == null) {
                    Entity.Scene.Add(v.Entity);
                    continue;
                }
                if (!v.Entity.IsInScene) {
                    Entity.Scene.Add(v.Entity);
                }
            }
        }
    }
}
