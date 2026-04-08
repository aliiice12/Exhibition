using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace ProjectExhibition.Event
{
    public class Schedule
    {
        [XmlElement("StartTime")]
        [JsonProperty("startTime")]
        public string StartTime { get; set; }
        [XmlElement("EndTime")]
        [JsonProperty("endTime")]
        public string EndTime { get; set; }
        [XmlElement("BreakTime")]
        [JsonProperty("breakTime")]
        public BreakTime BreakTime { get; set; }
        [XmlArray("WorkingDays")]
        [XmlArrayItem("Day")]
        [JsonProperty("workingDays")]
        public List<string> WorkingDays { get; set; } = new List<string>();
    }
}
