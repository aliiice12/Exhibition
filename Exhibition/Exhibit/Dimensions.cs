using Newtonsoft.Json;
using System.Xml.Serialization;
namespace ProjectExhibition.Exhibit
{
    public class Dimensions
    {
        [XmlElement("Height")]
        [JsonProperty("height")]
        public int Height { get; set; }
        [XmlElement("Width")]
        [JsonProperty("width")]
        public int Width { get; set; }
    }
}
