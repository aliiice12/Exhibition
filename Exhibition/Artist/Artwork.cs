using Newtonsoft.Json;
using System;
using System.Xml.Serialization;
namespace ProjectExhibition.Artist
{
    public class Artwork
    {
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [XmlElement("title")]
        [JsonProperty("title")]
        public string Title { get; set; }
        [XmlElement("year")]
        [JsonProperty("year")]
        public int Year { get; set; }
    }
}
