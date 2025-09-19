using System.Text.Json.Serialization;

namespace Fellowship.SDK.Models
{
    public class Movie
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("runtimeInMinutes")]
        public int RuntimeInMinutes { get; set; }

        [JsonPropertyName("rottenTomatoesScore")]
        public double RottenTomatoesScore { get; set; }
    }
}