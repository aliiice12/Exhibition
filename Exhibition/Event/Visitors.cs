using Newtonsoft.Json;
using System.Xml.Serialization;
namespace ProjectExhibition.Event
{
    public class Visitors
    {
        [XmlElement("expectedNumber")]
        [JsonProperty("expectedNumber")]
        public int ExpectedNumber { get; set; }
        [XmlElement("ageLimit")]
        [JsonProperty("ageLimit")]
        public int AgeLimit { get; set; }
    }
}
