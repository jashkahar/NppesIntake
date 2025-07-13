using System.Text.Json.Serialization;

namespace NppesIntake.Core.DTOs;

public class NppesApiResponse
{
    [JsonPropertyName("result_count")]
    public int ResultCount { get; set; }

    [JsonPropertyName("results")]
    public List<NppesResultDto> Results { get; set; } = new();
}