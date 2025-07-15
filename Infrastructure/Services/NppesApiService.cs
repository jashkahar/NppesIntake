using Microsoft.Extensions.Logging;
using NppesIntake.Core.DTOs;
using NppesIntake.Core.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace NppesIntake.Infrastructure.Services;

public class NppesApiService : INppesApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NppesApiService> _logger;
    private const string BaseUrl = "https://npiregistry.cms.hhs.gov/api/";

    // The constructor now takes IHttpClientFactory
    public NppesApiService(IHttpClientFactory httpClientFactory, ILogger<NppesApiService> logger)
    {
        // We create the specific client we need by its name
        _httpClient = httpClientFactory.CreateClient("NppesApiClient");
        _logger = logger;
    }

    public async Task<IEnumerable<NpiDataRecord>> SearchByNpiAsync(string npiNumber)
    {
        var url = $"{BaseUrl}?number={npiNumber}&version=2.1";
        return await FetchAndMapAsync(url);
    }

    public async Task<IEnumerable<NpiDataRecord>> SearchByNameAsync(string firstName, string lastName)
    {
        var url = $"{BaseUrl}?first_name={firstName}&last_name={lastName}&version=2.1";
        return await FetchAndMapAsync(url);
    }

    private async Task<IEnumerable<NpiDataRecord>> FetchAndMapAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<NppesApiResponse>(url);
            if (response?.Results == null)
                return Enumerable.Empty<NpiDataRecord>();
            return response.Results.Select(MapToNpiDataRecord).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching NPPES data");
            return Enumerable.Empty<NpiDataRecord>();
        }
    }

    private NpiDataRecord MapToNpiDataRecord(NppesResultDto dto)
    {
        var address = dto.Addresses?.FirstOrDefault(a => a.AddressPurpose == "LOCATION");
        return new NpiDataRecord
        {
            Npi = dto.Number,
            EnumerationType = dto.EnumerationType,
            FirstName = dto.Basic?.FirstName,
            LastName = dto.Basic?.LastName,
            Credential = dto.Basic?.Credential,
            OrganizationName = dto.Basic?.OrganizationName,
            LocationAddress = address == null ? null : new NpiDataRecord.Address
            {
                Address1 = address.Address1,
                Address2 = address.Address2,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                PhoneNumber = address.TelephoneNumber
            }
        };
    }

    // Private DTOs for deserialization
    private class NppesApiResponse
    {
        [JsonPropertyName("results")]
        public List<NppesResultDto>? Results { get; set; }
    }

    private class NppesResultDto
    {
        [JsonPropertyName("number")]
        public long Number { get; set; }
        [JsonPropertyName("enumeration_type")]
        public string EnumerationType { get; set; } = string.Empty;
        [JsonPropertyName("basic")]
        public NppesBasicDto? Basic { get; set; }
        [JsonPropertyName("addresses")]
        public List<NppesAddressDto>? Addresses { get; set; }
    }

    private class NppesBasicDto
    {
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }
        [JsonPropertyName("credential")]
        public string? Credential { get; set; }
        [JsonPropertyName("organization_name")]
        public string? OrganizationName { get; set; }
    }

    private class NppesAddressDto
    {
        [JsonPropertyName("address_purpose")]
        public string? AddressPurpose { get; set; }
        [JsonPropertyName("address_1")]
        public string Address1 { get; set; } = string.Empty;
        [JsonPropertyName("address_2")]
        public string? Address2 { get; set; }
        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;
        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; } = string.Empty;
        [JsonPropertyName("telephone_number")]
        public string? TelephoneNumber { get; set; }
    }
}