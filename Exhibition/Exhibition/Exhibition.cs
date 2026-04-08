using Newtonsoft.Json;
using ProjectExhibition.Artist;
using ProjectExhibition.Exhibit;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace ProjectExhibition
{
    [XmlRoot("exhibition")]
    public class Exhibition
    {
        [XmlElement("exhibit")]
        [JsonProperty("exhibit")]
        public List<ExhibitionExhibit> Exhibits { get; set; } = new List<ExhibitionExhibit>();
        [XmlElement("event")]
        [JsonProperty("event")]
        public List<Event.Event> Events { get; set; } = new List<Event.Event>();
        [XmlElement("artist")]
        [JsonProperty("artist")]
        public List<ExhibitionArtist> Artists { get; set; } = new List<ExhibitionArtist>();
    }
}
