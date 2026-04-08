using Newtonsoft.Json;
using System.Xml.Serialization;
namespace ProjectExhibition.Exhibit
{
    public class Security
    {
        [XmlElement("Level")]
        [JsonProperty("level")]
        public string Level { get; set; }
        [XmlElement("Guard")]
        [JsonProperty("guard")]
        public string Guard { get; set; }
    }
}
