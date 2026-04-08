using Newtonsoft.Json;
using System.Xml.Serialization;
namespace ProjectExhibition.Artist
{
    public class Biography
    {
        [XmlElement("BirthPlace")]
        [JsonProperty("birthPlace")]
        public string BirthPlace { get; set; }
        [XmlElement("Education")]
        [JsonProperty("education")]
        public Education Education { get; set; }
    }
}
