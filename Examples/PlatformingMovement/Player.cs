using Otter.Core;
using Otter.Components;
using Otter.Graphics;
using Otter.Graphics.Drawables;
using Otter.Components.Movement;

namespace PlatformingExample
{
    class Player : Entity
    {

        Image imageRect = Image.CreateRectangle(10, 24, Color.Yellow);

        public Player(float x, float y) : base(x, y)
        {
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

        public override void Render()
        {
            base.Render();

            // Render the hitbox for educational purposes. ;D
            Hitbox.Render();
        }
    }
}
