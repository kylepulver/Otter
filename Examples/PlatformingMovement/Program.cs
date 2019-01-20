using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformingExample {
    class Program {
        static void Main(string[] args) {
            // Quick example on how to use PlatformingMovement.cs

            // Make a new game.
            var game = new Game("Platformer Example");

            // Start the game with a new PlatformerScene.
            game.Start(new PlatformerScene());
        }
    }

    class PlatformerScene : Scene {

        public PlatformerScene() : base() {
            // Create the Ogmo Editor project.
            var ogmoProject = new OgmoProject("OgmoProject.oep");

            // Register the "Solid" layer with the tag Solid.
            ogmoProject.RegisterTag(CollisionTag.Solid, "Solid");

            // Set the game's color to the Ogmo Project's background color.
            Game.Instance.Color = ogmoProject.BackgroundColor;

            // Load the level.
            ogmoProject.LoadLevel("Level.oel", this);
        }
    }

    class Player : Entity {

        Image imageRect = Image.CreateRectangle(10, 24, Color.Yellow);

        public Player(float x, float y) : base(x, y) {
            // Add a simple graphic.
            AddGraphic(imageRect);
            imageRect.CenterOrigin();

            // Add a hitbox and center it.
            SetHitbox(10, 24);
            Hitbox.CenterOrigin();

            // Create controls to use for platforming movement.
            var axis = Axis.CreateArrowKeys();
            var jumpButton = new Button().AddKey(Key.Space);

            // Create the platforming movement and adjust some values.
            var platformingMovement = new PlatformingMovement(300, 1000, 15);
            platformingMovement.JumpStrength = 500;
            platformingMovement.Acceleration[AccelType.Ground] = 100;
            platformingMovement.Acceleration[AccelType.Air] = 10;

            // Register the controls with the platforming movement.
            platformingMovement.JumpButton = jumpButton;
            platformingMovement.Axis = axis;

            // Register the Solid tag as a collidable surface.
            platformingMovement.AddCollision(CollisionTag.Solid);

            // Register the Entity's hitbox as the platforming collider.
            platformingMovement.Collider = Hitbox;

            // Add all the components.
            AddComponents(
                axis,
                jumpButton,
                platformingMovement
                );

            /*
             * ======== Notes
             * INPUT:
             * The platforming movement component relies on an axis for movement, and
             * a button for jumping.  The axis and button are not updated by the
             * platforming movement, so they must be updated from another source.
             * Adding the axis and jump button to the Entity ensures that they are
             * updated.
             * 
             * COLLIDING WITH STUFF:
             * The AddCollision method will register a tag with the platforming
             * movement to use as solid ground.  There is also a method for registering
             * a tag as a jump through platform.
             * 
             * The platforming movement also needs a Collider to be set to its Collider
             * field.  This will be the collider that the platforming movement uses
             * as the primary collider when checking for solids and jump through
             * platforms.
             */
        }

        public override void Render() {
            base.Render();

            // Render the hitbox for educational purposes. ;D
            Hitbox.Render();
        }
    }

    /// <summary>
    /// Collision tags.
    /// </summary>
    public enum CollisionTag {
        Solid
    }
}
