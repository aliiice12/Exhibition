using Newtonsoft.Json;
using System.Xml.Serialization;
namespace ProjectExhibition.Event
{
    public class Location
    {
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public string Id { get; set; }
        [XmlElement("venue")]
        [JsonProperty("venue")]
        public string Venue { get; set; }
        [XmlElement("hall")]
        [JsonProperty("hall")]
        public string Hall { get; set; }
    }
}
