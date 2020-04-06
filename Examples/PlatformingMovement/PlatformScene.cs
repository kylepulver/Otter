using Otter.Core;
using Otter.Utility;

namespace PlatformingExample
{
    class PlatformerScene : Scene
    {
        public PlatformerScene() : base()
        {
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
}
