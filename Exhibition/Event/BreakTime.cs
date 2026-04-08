using Newtonsoft.Json;
using System.Xml.Serialization;
namespace ProjectExhibition.Event
{
    public class BreakTime
    {
        [XmlElement("From")]
        [JsonProperty("from")]
        public string From { get; set; }
        [XmlElement("To")]
        [JsonProperty("to")]
        public string To { get; set; }
    }
}
