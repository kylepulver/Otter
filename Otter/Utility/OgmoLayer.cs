using System.Xml;

using Otter.Graphics;

namespace Otter.Utility
{
    /// <summary>
    /// Class representing a layer loaded from Ogmo.
    /// </summary>
    public class OgmoLayer
    {
        #region Public Fields

        /// <summary>
        /// The name of the layer.
        /// </summary>
        public string Name;

        /// <summary>
        /// The export mode of the layer from Ogmo Editor.
        /// </summary>
        public string ExportMode;

        /// <summary>
        /// The type of the layer from Ogmo Editor.
        /// </summary>
        public string Type;

        /// <summary>
        /// The width of each grid cell.
        /// </summary>
        public int GridWidth;

        /// <summary>
        /// The height of each grid cell.
        /// </summary>
        public int GridHeight;

        /// <summary>
        /// The horizontal parallax of the layer.
        /// </summary>
        public float ScrollX = 1;

        /// <summary>
        /// The vertical parallax of the layer.
        /// </summary>
        public float ScrollY = 1;

        /// <summary>
        /// The color of layer from Ogmo Editor.
        /// </summary>
        public Color Color;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new OgmoLayer.
        /// </summary>
        /// <param name="name">The name of the layer.</param>
        /// <param name="type">The type of the layer.</param>
        public OgmoLayer(string name, string type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Create a new OgmoLayer by parsing an XmlElement.
        /// </summary>
        /// <param name="xml">An XmlElement from an Ogmo Editor project file.</param>
        public OgmoLayer(XmlElement xml)
        {
            Name = xml["Name"].InnerText;
            Type = xml.Attributes["xsi:type"].Value;

            GridWidth = int.Parse(xml["Grid"]["Width"].InnerText);
            GridHeight = int.Parse(xml["Grid"]["Height"].InnerText);

            ScrollX = int.Parse(xml["ScrollFactor"]["X"].InnerText);
            ScrollY = int.Parse(xml["ScrollFactor"]["Y"].InnerText);

            if (Type == "GridLayerDefinition")
            {
                Color = new Color(xml["Color"]);
            }
        }

        #endregion
    }
}
