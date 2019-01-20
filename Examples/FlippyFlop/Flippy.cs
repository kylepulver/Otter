using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippyFlop {
    class Flippy : Entity {

        public Image Image = Image.CreateRectangle(20, Color.Yellow);

        public Session Session;

        public Movement Movement = new Movement();

        public int GravityDirection = -1;

        public int SpeedY = 0;

        public bool Dead;

        public float GravityForce = 0;

        public Flippy(Session session) : base() {
            Session = session;

            Image.OutlineThickness = 3;
            Image.OutlineColor = Color.Orange;

            Graphic = Image;

            SetHitbox(10, 10);
            Hitbox.CenterOrigin();
            Image.CenterOrigin();

            AddComponent(Movement);
            Movement.Collider = Hitbox;

            Y = Game.Instance.HalfWidth;
            X = 120;

           
        }

        public override void Added() {
            base.Added();
            Tween(Image, new { ScaleX = 1, ScaleY = 1 }, 30).From(new { ScaleX = 0, ScaleY = 0 }).Ease(Ease.ElasticOut);
            Tween(this, new { GravityForce = 30 }, 30);
        }

        public override void Update() {
            base.Update();

            if (!Dead) {
                if (Session.Controller.A.Pressed) {
                    EventRouter.Publish(Events.FlippyFlipped);
                    Tween(Image, new { ScaleX = 1, ScaleY = 1 }, 45).From(new { ScaleX = 2f, ScaleY = 0.5f }).Ease(Ease.ElasticOut);
                    Tween(Image, new { Angle = 0 }, 30).From(new { Angle = Rand.Float(-10, 10) });
                    GravityDirection *= -1;
                    SpeedY /= 4;
                }

                Scene.Add(new FlippyTrail(X, Y));
                Scene.Add(new FlippyTrail(X, Y + SpeedY * 0.005f));

                SpeedY += (int)(GravityDirection * GravityForce);
                SpeedY = (int)Util.Clamp(SpeedY, -1000, 1000);

                Movement.MoveY(SpeedY);


                if (Overlap(X, Y, (int)Tags.Wall)) {
                    Image.Color = Color.Red;
                    EventRouter.Publish(Events.FlippyDied);
                    Death();
                }
                if (Y < 50) {
                    EventRouter.Publish(Events.FlippyDied);
                    Death();
                }
                if (Y > Game.Instance.Height - 50) {
                    EventRouter.Publish(Events.FlippyDied);
                    Death();
                }
            }
            else {
                Image.Angle += 15;
            }
            
            

        }

        public void Death() {
            if (Dead) return;
            Image.Color = Color.Red;
            Image.OutlineColor = Color.Black;
            Tween(Image, new { ScaleX = 0, ScaleY = 0}, 30).From(new { ScaleX = 1.5f, ScaleY = 1.5f}).Ease(Ease.BackIn).OnComplete(() => { RemoveSelf(); });
            Dead = true;
        }

   
        
    }
}
