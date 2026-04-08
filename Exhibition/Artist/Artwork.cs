using Newtonsoft.Json;
using System.Xml.Serialization;
namespace ProjectExhibition.Artist
{
    public class Artwork
    {
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public string Id { get; set; }
        [XmlElement("title")]
        [JsonProperty("title")]
        public string Title { get; set; }
        [XmlElement("year")]
        [JsonProperty("year")]
        public int Year { get; set; }
    }
}
