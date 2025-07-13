using System.Text.Json.Serialization;

namespace NppesIntake.Core.DTOs;

public class NppesBasicDto
{
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    [JsonPropertyName("middle_name")]
    public string MiddleName { get; set; }

    [JsonPropertyName("credential")]
    public string Credential { get; set; }

    [JsonPropertyName("sole_proprietor")]
    public string SoleProprietor { get; set; }

    [JsonPropertyName("sex")]
    public string Gender { get; set; }

    [JsonPropertyName("enumeration_date")]
    public string EnumerationDate { get; set; }

    [JsonPropertyName("last_updated")]
    public string LastUpdated { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("organization_name")]
    public string OrganizationName { get; set; }
}