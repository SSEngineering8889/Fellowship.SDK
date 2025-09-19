using System.Text.Json.Serialization;

namespace Fellowship.SDK.Models;

public class Quote
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("dialog")]
    public string Dialog { get; set; } = default!;

    [JsonPropertyName("character")]
    public string Character { get; set; } = default!;

    [JsonPropertyName("movie")]
    public string Movie { get; set; } = default!;
}
