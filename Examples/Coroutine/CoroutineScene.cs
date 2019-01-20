using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoroutineExample {
    class CoroutineScene : Scene {

        public Image ImageBox = Image.CreateRectangle(50);

        public Color NextColor = Color.White;
        public Color CurrentColor = Color.White;

        public CoroutineScene() : base() {
            // Center that box.
            ImageBox.CenterOrigin();

            // Gotta draw the box.
            AddGraphic(ImageBox);

            // Set the box position.
            ImageBox.X = 100;
            ImageBox.Y = 100;
        }

        public override void Begin() {
            base.Begin();

            // Start the coroutine, yo.
            Game.Coroutine.Start(MainRoutine());
        }

        /// <summary>
        /// The main coroutine to execute.  This will move the box around and change its color.
        /// </summary>
        /// <returns>Whatever a coroutine thing returns.  Sometimes 0 I guess.</returns>
        IEnumerator MainRoutine() {
            // Wait for 30 frames.
            yield return Coroutine.Instance.WaitForFrames(30);
            // Set the next color.
            NextColor = Color.Red;
            // Move the box to the top right.
            yield return MoveBoxTo(540, 100);

            // Wait for 30 frames.
            yield return Coroutine.Instance.WaitForFrames(30);
            // Set the next color.
            NextColor = Color.Yellow;
            // Move the box to the bottom right.
            yield return MoveBoxTo(540, 380);

            // Wait for 30 frames.
            yield return Coroutine.Instance.WaitForFrames(30);
            // Set the next color.
            NextColor = Color.Green;
            // Move the box to the bottom left.
            yield return MoveBoxTo(100, 380);

            // Wait for 30 frames.
            yield return Coroutine.Instance.WaitForFrames(30);
            // Set the next color.
            NextColor = Color.Cyan;
            // Move the box to the top left.
            yield return MoveBoxTo(100, 100);
            
            // Start a new coroutine.
            Game.Coroutine.Start(MainRoutine());
        }

        IEnumerator MoveBoxTo(float x, float y) {
            // Used to determine the completion.
            var initialDistance = Util.Distance(ImageBox.X, ImageBox.Y, x, y);

            float currentDistance = float.MaxValue;
            while (currentDistance > 1) {
                currentDistance = Util.Distance(ImageBox.X, ImageBox.Y, x, y);

                // Determine the completion of the movement from 0 to 1.
                var completion = Util.ScaleClamp(currentDistance, 0, initialDistance, 1, 0);

                // Lerp the color of the box.
                ImageBox.Color = Util.LerpColor(CurrentColor, NextColor, completion);

                // Spin the box along with its movement.
                ImageBox.Angle = Util.ScaleClamp(completion, 0, 1, 0, 360);

                // Actually move the box.
                ImageBox.X = Util.Approach(ImageBox.X, x, 5);
                ImageBox.Y = Util.Approach(ImageBox.Y, y, 5);

                // Wait until next frame.
                yield return 0;
            }

            // Done moving.  Update the color.
            CurrentColor = NextColor;
        }
    }
}
