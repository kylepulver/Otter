using System.IO;

using Otter.Utility;

namespace Otter.Graphics.Text
{
    public class Font : BaseFont
    {

        public Font(string source)
        {
            font = Fonts.Load(source);
        }

        public Font(Stream stream)
        {
            font = Fonts.Load(stream);
        }

        public Font()
        {
            font = Fonts.DefaultFont;
        }

        public override float GetKerning(char first, char second, int characterSize)
        {
            return font.GetKerning((uint)first, (uint)second, (uint)characterSize);
        }
    }
}
