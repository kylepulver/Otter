using System.Collections.Generic;
using System.Xml;

using Otter.Graphics;

namespace Otter.Utility
{
    /// <summary>
    /// A simple data class that just extends Dictionary.
    /// </summary>
    public class OgmoData : Dictionary<string, string>
    {

        public OgmoData(XmlAttributeCollection attributes)
        {
            foreach (XmlAttribute attr in attributes)
            {
                Add(attr.Name, attr.Value);
            }
        }

        public int GetInt(string key, int onNull)
        {
            return this.ValueAsInt(key, onNull);
        }

        public bool GetBool(string key, bool onNull)
        {
            return this.ValueAsBool(key, onNull);
        }

        public float GetFloat(string key, float onNull)
        {
            return this.ValueAsFloat(key, onNull);
        }

        public Color GetColor(string key, Color onNull)
        {
            return this.ValueAsColor(key, onNull);
        }
    }
}
