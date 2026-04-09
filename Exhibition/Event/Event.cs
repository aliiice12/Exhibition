using Newtonsoft.Json;
using System;
using System.Xml.Serialization;
namespace ProjectExhibition.Event
{
    public class Event
    {
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [XmlElement("name")]
        [JsonProperty("name")]
        public string Name { get; set; }
        [XmlElement("date")]
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [XmlElement("city")]
        [JsonProperty("city")]
        public string City { get; set; }
        [XmlElement("location")]
        [JsonProperty("location")]
        public Location Location { get; set; }
        [XmlElement("organizer")]
        [JsonProperty("organizer")]
        public Organizer Organizer { get; set; }
        [XmlElement("visitors")]
        [JsonProperty("visitors")]
        public Visitors Visitors { get; set; }
    }
}
