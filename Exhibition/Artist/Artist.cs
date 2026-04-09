using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace ProjectExhibition.Artist
{
    public class ExhibitionArtist
    {
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [XmlElement("name")]
        [JsonProperty("name")]
        public string Name { get; set; }
        [XmlElement("country")]
        [JsonProperty("country")]
        public string Country { get; set; }
        [XmlElement("birthYear")]
        [JsonProperty("birthYear")]
        public int BirthYear { get; set; }
        [XmlElement("deathYear")]
        [JsonProperty("deathYear")]
        public int DeathYear { get; set; }
        [XmlArray("artworks")]
        [XmlArrayItem("artwork")]
        [JsonProperty("artworks")]
        public List<Artwork> Artworks { get; set; } = new List<Artwork>();
    }
}

