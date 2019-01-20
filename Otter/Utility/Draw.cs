using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Class used for rendering graphics.
    /// </summary>
    public class Draw {

        #region Static Fields

        static internal SpriteBatch SpriteBatch = new SpriteBatch();

        static internal RenderStates renderStates = RenderStates.Default;

        static RectangleShape tempRect = new RectangleShape();
        static CircleShape tempCircle = new CircleShape();

        #endregion

        #region Static Methods

        static internal void Drawable(Drawable drawable, RenderStates states) {
            SpriteBatch.End();
            Target.Draw(drawable, states);

            if (Game.Instance.countRendering) Game.Instance.RenderCount++;
        }

        static internal void Drawable(Vertex[] vertices, RenderStates states) {
            SpriteBatch.End();
            Target.Draw(vertices, states);

            if (Game.Instance.countRendering) Game.Instance.RenderCount++;
        }

        static internal void Spritebatch(VertexArray vertices, RenderStates states) {
            Target.Draw(vertices, states);

            if (Game.Instance.countRendering) Game.Instance.RenderCount++;
        }

        static internal void Batchable(VertexArray vertices, RenderStates states) {
            SpriteBatch.Begin();
            SpriteBatch.Draw(vertices, states);
        }

        /// <summary>
        /// Renders a Graphic to the current target Surface.
        /// </summary>
        /// <param name="graphic">The Graphic to render.</param>
        /// <param name="x">The x offset to position the Graphic at.</param>
        /// <param name="y">The y offset to position the Graphic at.</param>
        static public void Graphic(Graphic graphic, float x = 0, float y = 0) {
            graphic.Render(x, y);
        }

        /// <summary>
        /// Renders an Entity.
        /// </summary>
        /// <param name="e">The Entity to render.</param>
        public static void Entity(Entity e) {
            e.RenderInternal();
        }

        /// <summary>
        /// Renders an Entity at a specified X Y position.
        /// </summary>
        /// <param name="e">The Entity to render.</param>
        /// <param name="x">The X position to place the Entity for rendering.</param>
        /// <param name="y">The Y position to place the Entity for rendering.</param>
        public static void Entity(Entity e, float x = 0, float y = 0) {
            var tempX = e.X;
            var tempY = e.Y;
            e.SetPosition(x, y);
            Entity(e);
            e.SetPosition(tempX, tempY);
        }

        /// <summary>
        /// Draws simple Text.  Should only be used for debugging as this creates Text Graphics each time it's called!
        /// </summary>
        /// <param name="str">The string to display.</param>
        /// <param name="size">The size of the Text.</param>
        /// <param name="x">The X position to render the Text from.</param>
        /// <param name="y">The Y position to render the Text from.</param>
        public static void Text(string str, int size, float x = 0, float y = 0) {
            Draw.Graphic(new Text(str, size), x, y);
        }

        /// <summary>
        /// Renders a clipped Image to the current target Surface.
        /// </summary>
        /// <param name="image">the Image to render.</param>
        /// <param name="clip">The portion of the Image to render.</param>
        /// <param name="x">The x offset to position the Image at.</param>
        /// <param name="y">The y offset to position the Image at.</param>
        static public void ImageClip(Image image, Rectangle clip, float x = 0, float y = 0) {
            var tempRect = image.ClippingRegion;
            image.ClippingRegion = clip;

            image.Render(x, y);

            image.ClippingRegion = tempRect;
        }

        /// <summary>
        /// Draws an Image in parts to form a horizontally waving image.
        /// </summary>
        /// <param name="image">The image to draw.</param>
        /// <param name="step">How many steps to iterate through for the wave.</param>
        /// <param name="timer">The timer the wave should act with.</param>
        /// <param name="rate">The rate which the wave should move at.</param>
        /// <param name="amp">How far the wave will offset the image.</param>
        /// <param name="freq">How frequent the wave should repeat.</param>
        /// <param name="x">The x position to draw the image from.</param>
        /// <param name="y">The y position to draw the image from.</param>
        static public void ImageWaveX(Image image, int step, float timer, float rate, float amp, float freq, float x = 0, float y = 0) {
            for (var yy = 0; yy < image.Height; yy += step) {
                yy = (int)Util.Clamp(yy, image.Height);
                var xx = (int)Util.SinScale(timer * rate + yy * freq, -amp, amp);
                ImageClip(image, new Rectangle(0, yy, image.Width, step), x + xx, y);
            }
        }

        /// <summary>
        /// Draws an Image in parts to form a vertically waving image.
        /// </summary>
        /// <param name="image">The image to draw.</param>
        /// <param name="step">How many steps to iterate through for the wave.</param>
        /// <param name="timer">The timer the wave should act with.</param>
        /// <param name="rate">The rate which the wave should move at.</param>
        /// <param name="amp">How far the wave will offset the image.</param>
        /// <param name="freq">How frequent the wave should repeat.</param>
        /// <param name="x">The x position to draw the image from.</param>
        /// <param name="y">The y position to draw the image from.</param>
        static public void ImageWaveY(Image image, int step, float timer, float rate, float amp, float freq, float x = 0, float y = 0) {
            for (var xx = 0; xx < image.Width; xx += step) {
                xx = (int)Util.Clamp(xx, image.Width);
                var yy = (int)Util.SinScale(timer * rate + xx * freq, -amp, amp);
                ImageClip(image, new Rectangle(xx, 0, step, image.Height), x, y + yy);
            }
        }

        /// <summary>
        /// Change the Surface that is being rendered to.
        /// </summary>
        /// <param name="target">The new target Surface.</param>
        static public void SetTarget(Surface target) {
            if (Target != target) {
                SpriteBatch.End();
            }
            Target = target;
        }

        /// <summary>
        /// Reset the Surface that is being rendered to back to the default for the current Game.
        /// </summary>
        static public void ResetTarget() {
            if (Target != GameTarget) {
                SpriteBatch.End();
            }
            Target = GameTarget;
        }

        /// <summary>
        /// Draws a circle.  Recommended to use only for debugging purposes.
        /// </summary>
        /// <param name="x">The X position of the top left of the circle.</param>
        /// <param name="y">The Y position of the top left of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="fill">The fill color of the circle.</param>
        /// <param name="outline">The outline color of the circle.</param>
        /// <param name="outlineThickness">The outline thickness of the circle.</param>
        static public void Circle(float x, float y, int radius, Color fill = null, Color outline = null, float outlineThickness = 0) {
            tempCircle.Radius = radius;
            tempCircle.Position = new Vector2f(x, y);
            if (fill == null) {
                tempCircle.FillColor = Color.White.SFMLColor;
            }
            else {
                tempCircle.FillColor = fill.SFMLColor;
            }

            tempCircle.OutlineThickness = outlineThickness;

            if (outline == null) {
                tempCircle.OutlineColor = Color.None.SFMLColor;
            }
            else {
                tempCircle.OutlineColor = outline.SFMLColor;
            }

            Target.Draw(tempCircle);
        }

        /// <summary>
        /// Draws a circle.  Recommended to use only for debugging purposes.
        /// </summary>
        /// <param name="x">The X position of the top left of the circle.</param>
        /// <param name="y">The Y position of the top left of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The fill color of the circle.</param>
        static public void Circle(float x, float y, float radius, Color color) {
            tempCircle.Radius = radius;
            tempCircle.Position = new Vector2f(x, y);
            tempCircle.FillColor = color.SFMLColor;
            Target.Draw(tempCircle);
        }

        /// <summary>
        /// Draws a rectangle.  Recommended to use only for debugging purposes.
        /// </summary>
        /// <param name="x">The X position of the top left of the rectangle.</param>
        /// <param name="y">The Y position of the top left of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="fill">The fill color of the rectangle.</param>
        /// <param name="outline">The outline color of the rectangle.</param>
        /// <param name="outlineThickness">The outline thickness of the rectangle.</param>
        static public void Rectangle(float x, float y, float width, float height, Color fill = null, Color outline = null, float outlineThickness = 0) {
            tempRect.Size = new Vector2f(width, height);
            tempRect.Position = new Vector2f(x, y);
            if (outline == null) {
                tempRect.OutlineColor = Color.None.SFMLColor;
            }
            else {
                tempRect.OutlineColor = outline.SFMLColor;
            }
            tempRect.OutlineThickness = outlineThickness;

            if (fill == null) {
                tempRect.FillColor = Color.White.SFMLColor;
            }
            else {
                tempRect.FillColor = fill.SFMLColor;
            }

            Target.Draw(tempRect);
        }

        /// <summary>
        /// Draws a line using an OpenGL line.
        /// </summary>
        /// <param name="x1">The X position of the first point.</param>
        /// <param name="y1">The Y position of the first point.</param>
        /// <param name="x2">The X position of the second point.</param>
        /// <param name="y2">The Y position of the second point.</param>
        /// <param name="color">The color of the line.</param>
        static public void Line(float x1, float y1, float x2, float y2, Color color) {
            VertexArray vertices = new VertexArray(PrimitiveType.Lines);

            vertices.Append(new Vertex(new Vector2f(x1, y1), color.SFMLColor));
            vertices.Append(new Vertex(new Vector2f(x2, y2), color.SFMLColor));
            Drawable(vertices, RenderStates.Default);
        }

        /// <summary>
        /// Draws a line with a thickness using a quad.
        /// </summary>
        /// <param name="x1">The X position of the first point.</param>
        /// <param name="y1">The Y position of the first point.</param>
        /// <param name="x2">The X position of the second point.</param>
        /// <param name="y2">The Y position of the second point.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="thickness">The thickness of the line.</param>
        static public void Line(float x1, float y1, float x2, float y2, Color color, float thickness) {
            VertexArray vertices = new VertexArray(PrimitiveType.Quads);

            var line = new Vector2(x2 - x1, y2 - y1);
            var normalUp = new Vector2(y1 - y2, x2 - x1);
            var normalDown = new Vector2(y2 - y1, x1 - x2);

            normalUp.Normalize(thickness * 0.5f);
            normalDown.Normalize(thickness * 0.5f);

            float vx, vy;

            vx = (float)(x1 + normalUp.X);
            vy = (float)(y1 + normalUp.Y);

            vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));

            vx = (float)(x1 + normalDown.X);
            vy = (float)(y1 + normalDown.Y);

            vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));

            vx = (float)(x2 + normalDown.X);
            vy = (float)(y2 + normalDown.Y);

            vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));

            vx = (float)(x2 + normalUp.X);
            vy = (float)(y2 + normalUp.Y);

            vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));

            Drawable(vertices, RenderStates.Default);
        }

        /// <summary>
        /// Draws a line with rounded ends.
        /// </summary>
        /// <param name="x1">The X position of the first point.</param>
        /// <param name="y1">The Y position of the first point.</param>
        /// <param name="x2">The X position of the second point.</param>
        /// <param name="y2">The Y position of the second point.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="thickness">The thickness of the line.</param>
        static public void RoundedLine(float x1, float y1, float x2, float y2, Color color, float thickness) {
            VertexArray vertices = new VertexArray(PrimitiveType.TrianglesFan);

            int rotationSteps = 10;

            var line = new Vector2(x2 - x1, y2 - y1);
            var normalUp = new Vector2(y1 - y2, x2 - x1);
            var normalDown = new Vector2(y2 - y1, x1 - x2);

            normalUp.Normalize(thickness * 0.5f);
            normalDown.Normalize(thickness * 0.5f);

            var nextPoint = new Vector2();

            float vx, vy;

            vx = x1;
            vy = y1;

            vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));

            vx = (float)(x1 + normalUp.X);
            vy = (float)(y1 + normalUp.Y);

            vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));

            nextPoint.X = normalUp.X;
            nextPoint.Y = normalUp.Y;

            for (int i = 0; i < rotationSteps; i++) {
                nextPoint = Util.Rotate(nextPoint, -180 / rotationSteps);

                vx = (float)(x1 + nextPoint.X);
                vy = (float)(y1 + nextPoint.Y);

                vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));
            }

            vx = (float)(x1 + normalDown.X);
            vy = (float)(y1 + normalDown.Y);

            vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));

            vx = (float)(x2 + normalDown.X);
            vy = (float)(y2 + normalDown.Y);

            vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));

            for (int i = 0; i < rotationSteps; i++) {
                nextPoint = Util.Rotate(nextPoint, -180 / rotationSteps);

                vx = (float)(x2 + nextPoint.X);
                vy = (float)(y2 + nextPoint.Y);

                vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));
            }

            vx = (float)(x2 + normalUp.X);
            vy = (float)(y2 + normalUp.Y);

            vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));

            vx = (float)(x1 + normalUp.X);
            vy = (float)(y1 + normalUp.Y);

            vertices.Append(new Vertex(new Vector2f(vx, vy), color.SFMLColor));

            Drawable(vertices, RenderStates.Default);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The current target Surface to render to.
        /// </summary>
        static public Surface Target { get; internal set; }

        /// <summary>
        /// The surface that current Game is rendering to.
        /// </summary>
        static public Surface GameTarget { get; internal set; }

        #endregion


        
    }
}
