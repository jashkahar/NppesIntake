using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace NppesIntake.Core.DTOs;

public class NppesResultDto
{
    [JsonPropertyName("enumeration_type")]
    public string EnumerationType { get; set; }

    [JsonPropertyName("number")]
    public long Number { get; set; }

    [JsonPropertyName("last_updated_epoch")]
    public long LastUpdatedEpoch { get; set; }

    [JsonPropertyName("created_epoch")]
    public long CreatedEpoch { get; set; }

    [JsonPropertyName("basic")]
    public NppesBasicDto Basic { get; set; }

    [JsonPropertyName("addresses")]
    public List<NppesAddressDto> Addresses { get; set; }

    [JsonPropertyName("taxonomies")]
    public List<NppesTaxonomyDto> Taxonomies { get; set; }

    [JsonPropertyName("identifiers")]
    public List<NppesIdentifierDto> Identifiers { get; set; }
}