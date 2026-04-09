using Newtonsoft.Json;
using System;
using System.Xml.Serialization;
namespace ProjectExhibition.Event
{
    public class Organizer
    {
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [XmlElement("organization")]
        [JsonProperty("organization")]
        public string Organization { get; set; }
        [XmlElement("Schedule")]
        [JsonProperty("schedule")]
        public Schedule Schedule { get; set; }
    }
}
