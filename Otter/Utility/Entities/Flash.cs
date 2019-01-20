namespace Otter {
    /// <summary>
    /// Entity that acts as a screen flash.  Best used when using the constructor that allows for
    /// initial parameters be set:
    /// <example>
    /// Flash(Color.Red) { Alpha = 0.5, Blend = BlendMode.Add };
    /// </example>
    /// </summary>
    public class Flash : Entity {

        #region Static Fields

        /// <summary>
        /// The default life span for all created Flash Entities.
        /// </summary>
        public static int DefaultLifeSpan = 60;

        #endregion

        #region Private Fields

        Image imgFlash;

        #endregion

        #region Public Fields

        /// <summary>
        /// The Color for the Flash.
        /// </summary>
        public Color Color;

        /// <summary>
        /// The initial alpha for the Flash.
        /// </summary>
        public float Alpha = 1;

        /// <summary>
        /// The final alpha for the Flash.
        /// </summary>
        public float FinalAlpha = 0;

        /// <summary>
        /// The BlendMode for the Flash.
        /// </summary>
        public BlendMode Blend = BlendMode.Alpha;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Flash.
        /// </summary>
        /// <param name="color">The Color of the Flash.</param>
        public Flash(Color color) : base(0, 0) {
            Color = color;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Added to the Scene.
        /// </summary>
        public override void Added() {
            base.Added();

            if (LifeSpan == 0) {
                LifeSpan = DefaultLifeSpan;
            }

            imgFlash = Image.CreateRectangle(Game.Instance.Width, Game.Instance.Height, Color);
            imgFlash.Blend = Blend;
            imgFlash.Scroll = 0;
            imgFlash.CenterOriginZero();
            if (Surface != null) {
                imgFlash.Scale = 1 / Surface.CameraZoom;
            }
            else {
                imgFlash.Scale = 1 / Game.Surface.CameraZoom;
            }
            SetGraphic(imgFlash);
        }

        /// <summary>
        /// Updated.
        /// </summary>
        public override void Update() {
            base.Update();

            if (Surface != null) {
                imgFlash.Scale = 1 / Surface.CameraZoom;
            }
            else {
                imgFlash.Scale = 1 / Game.Surface.CameraZoom;
            }

            imgFlash.Alpha = Util.ScaleClamp(Timer, 0, LifeSpan, Alpha, FinalAlpha);
        }

        /// <summary>
        /// Removed from the Scene.
        /// </summary>
        public override void Removed() {
            base.Removed();

            ClearGraphics();
        }

        #endregion

    }
}
