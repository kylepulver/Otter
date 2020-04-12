using SFML.Graphics;

using Otter.Utility;

namespace Otter.Graphics.Text
{
    public abstract class BaseFont
    {
        internal SFML.Graphics.Font font;

        public BaseFont()
        {
            font = Fonts.DefaultFont;
        }

        internal virtual Glyph GetGlyph(char c, int size, bool bold)
        {
            return font.GetGlyph((uint)c, (uint)size, bold, 1f);
        }

        internal virtual float GetLineSpacing(int size)
        {
            return font.GetLineSpacing((uint)size);
        }

        internal virtual Texture GetTexture(int size)
        {
            return new Texture(font.GetTexture((uint)size));
        }

        public virtual float GetKerning(char first, char second, int characterSize)
        {
            return 0;
        }
    }
}
