using Newtonsoft.Json;
using System.Xml.Serialization;
namespace ProjectExhibition.Exhibit
{
    public class ExhibitLocation
    {
        [XmlElement("hall")]
        [JsonProperty("hall")]
        public string Hall { get; set; }
        [XmlElement("stand")]
        [JsonProperty("stand")]
        public int Stand { get; set; }
    }
}
