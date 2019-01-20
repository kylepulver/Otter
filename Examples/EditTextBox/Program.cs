using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditBox {
    class Program {
        static void Main(string[] args) {
            // Create an Otter game with default parameters.
            var game = new Game();
            // Set the background color.
            game.Color = new Color(0.2f, 0.2f, 0.5f);
            // Want to show the mouse for this example.
            game.MouseVisible = true;

            // Create a new scene.
            var scene = new Scene();

            // Add some text edit boxes to the scene at various positions.
            scene.Add(new TextEditBox(100, 100));
            scene.Add(new TextEditBox(100, 200));
            scene.Add(new TextEditBox(100, 300));

            // Start up the game using the scene we just made.
            game.Start(scene);
        }
    }

    class TextEditBox : Entity {

        // The text graphic to display.
        public Text Text = new Text();
        // The background image to display the text on.
        public Image ImageBox = Image.CreateRectangle(400, 20);

        // Wether or not the entity has focus or not.
        public bool HasFocus;

        // The size limit of the string.
        public int CharacterLimit = 44;

        // The string inputted into the box.
        public string InputString = "";

        public TextEditBox(float x, float y) : base(x, y) {
            // Configure the box for the text.
            ImageBox.OutlineColor = Color.Black;
            ImageBox.OutlineThickness = 3;

            // Set the color of the text to black.
            Text.Color = Color.Black;

            // Add the graphics.
            AddGraphic(ImageBox);
            AddGraphic(Text);
        }

        public override void Update() {
            base.Update();

            // If the text box currently has focus then set the Text's string to the Input KeyString.
            if (HasFocus) {
                InputString = Input.KeyString;
                // If we exceed the character limit...
                if (InputString.Length > CharacterLimit) {
                    // Only keep the characters from under the limit.
                    InputString = InputString.Substring(0, CharacterLimit);
                    // Then set the KeyString to get rid of characters beyond the limit.
                    Input.KeyString = InputString;
                }

                // If the character limit isn't reached...
                if (InputString.Length < CharacterLimit) {
                    // Display a blinking pipe character.
                    if (Timer % 30 >= 15) {
                        Text.String = InputString + "|";
                    }
                    else {
                        Text.String = InputString;
                    }
                }
                else {
                    // If we're at the limit just show the string
                    Text.String = InputString;
                }
            }
            else {
                // If no focus then just display the string normally.
                Text.String = InputString;
            }

            // If the mouse button left is pressed check if I was clicked on.
            if (Input.MouseButtonPressed(MouseButton.Left)) {
                if (Util.InRect(Scene.MouseX, Scene.MouseY, X, Y, 400, 50)) {
                    if (!HasFocus) {
                        // If I was clicked on, and I didn't have focus, then I now have focus.
                        Focus();
                    }
                }
                else {
                    if (HasFocus) {
                        // If I wasn't clicked on, and I had focus, then I don't have focus.
                        Unfocus();
                    }
                }
            }

            if (HasFocus) {
                // If we have focus check for the return key.
                if (Input.KeyPressed(Key.Return)) {
                    // If pressed then unfocus.
                    Unfocus();
                }
            }
        }

        public void Focus() {
            // Set focus to true.
            HasFocus = true;
            // When this object first gets focus set the Input.KeyString to the current string of the text.
            Input.KeyString = InputString;
            // Also make the outline color fancy yay.
            ImageBox.OutlineColor = Color.Yellow;
        }

        public void Unfocus() {
            // Set focus to false.
            HasFocus = false;
            // Trim the string (in case we added a \n from the return.)
            InputString = InputString.Trim();
            // Restore the outline color to black.
            ImageBox.OutlineColor = Color.Black;
        }

    }
}
