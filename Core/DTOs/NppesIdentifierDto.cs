using System.Text.Json.Serialization;

namespace NppesIntake.Core.DTOs;

public class NppesIdentifierDto
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("desc")]
    public string Description { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("issuer")]
    public string Issuer { get; set; }
}