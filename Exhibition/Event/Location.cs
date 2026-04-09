using Newtonsoft.Json;
using System;
using System.Xml.Serialization;
namespace ProjectExhibition.Event
{
    public class Location
    {
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [XmlElement("venue")]
        [JsonProperty("venue")]
        public string Venue { get; set; }
        [XmlElement("hall")]
        [JsonProperty("hall")]
        public string Hall { get; set; }
    }
}
