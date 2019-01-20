using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class Countdown : Component {

        public float Max;
        public float Min;
        public float Decrement = -1;
        public float Value;

        public bool IsCompleted {
            get { return Value <= 0; }
        }

        public float Completion {
            get {
                return Util.Clamp((Max - Value) / Max, 0, 1);
            }
        }

        public Action OnTrigger = delegate { };
        public bool Triggered;

        public Countdown(float max) {
            Max = max;
            Value = max;
        }

        public Countdown(float max, float value) : this(max) {
            Value = value;
        }

        public override void Update() {
            base.Update();
            Tick();
        }

        public void Tick() {
            Value += Decrement;
            if (IsCompleted) {
                Value = 0;
                if (!Triggered) {
                    Triggered = true;
                    OnTrigger();
                }
            }
        }

        public void Reset() {
            Value = Max;
            Triggered = false;
        }

        
    }
}
