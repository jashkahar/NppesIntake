using System.Text.Json.Serialization;

namespace NppesIntake.Core.DTOs;

public class NppesTaxonomyDto
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("desc")]
    public string Description { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("license")]
    public string License { get; set; }

    [JsonPropertyName("primary")]
    public bool IsPrimary { get; set; }
}