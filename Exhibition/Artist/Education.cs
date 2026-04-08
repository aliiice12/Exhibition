using Newtonsoft.Json;
using System.Xml.Serialization;
namespace ProjectExhibition.Artist
{
    public class Education
    {
        [XmlElement("Master")]
        [JsonProperty("master")]
        public string Master { get; set; }
        [XmlElement("City")]
        [JsonProperty("city")]
        public string City { get; set; }
    }
}
