using SFML.Graphics;
using System;
using System.IO;

namespace Otter {
    public class Font : BaseFont {

        public Font(string source) {
            font = Fonts.Load(source);
        }

        public Font(Stream stream) {
            font = Fonts.Load(stream);
        }

        public Font() {
            font = Fonts.DefaultFont;
        }

        public override float GetKerning(char first, char second, int characterSize) {
            return font.GetKerning((uint)first, (uint)second, (uint)characterSize);
        }
    }

    public abstract class BaseFont {
        internal SFML.Graphics.Font font;

        public BaseFont() {
            font = Fonts.DefaultFont;
        }

        internal virtual Glyph GetGlyph(char c, int size, bool bold) {
            return font.GetGlyph((uint)c, (uint)size, bold);
        }

        internal virtual float GetLineSpacing(int size) {
            return font.GetLineSpacing((uint)size);
        }

        internal virtual Texture GetTexture(int size) {
            return new Texture(font.GetTexture((uint)size));
        }

        public virtual float GetKerning(char first, char second, int characterSize) {
            return 0;
        }

    }
}
