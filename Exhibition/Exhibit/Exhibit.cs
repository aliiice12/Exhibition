using Newtonsoft.Json;
using System.Xml.Serialization;
namespace ProjectExhibition.Exhibit
{
    public class ExhibitionExhibit
    {
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public string Id { get; set; }
        [XmlElement("title")]
        [JsonProperty("title")]
        public string Title { get; set; }

        [XmlElement("type")]
        [JsonProperty("type")]
        public string Type { get; set; }
        [XmlElement("year")]
        [JsonProperty("year")]
        public int Year { get; set; }
        [XmlElement("material")]
        [JsonProperty("material")]
        public string Material { get; set; }
        [XmlElement("exhibitPosition")]
        [JsonProperty("exhibitPosition")]
        public ExhibitLocation ExhibitLocation { get; set; }
        [XmlElement("dimensions")]
        [JsonProperty("dimensions")]
        public Dimensions Dimensions { get; set; }
        [XmlElement("security")]
        [JsonProperty("security")]
        public Security Security { get; set; }
    }
}
