using Microsoft.Extensions.Logging;
using NppesIntake.Core.DTOs;
using NppesIntake.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NppesIntake.Infrastructure.Services;

public class NppesApiService : INppesApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NppesApiService> _logger;

    public NppesApiService(IHttpClientFactory httpClientFactory, ILogger<NppesApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("NppesApiClient");
        _logger = logger;
    }

    public Task<IEnumerable<NppesResultDto>> SearchByNpiAsync(string npiNumber)
    {
        var parameters = new Dictionary<string, string> { { "number", npiNumber } };
        return SearchNppesApiAsync(parameters);
    }

    public Task<IEnumerable<NppesResultDto>> SearchByNameAsync(string firstName, string lastName)
    {
        var parameters = new Dictionary<string, string>
        {
            { "first_name", firstName },
            { "last_name", lastName }
        };
        return SearchNppesApiAsync(parameters);
    }

    private async Task<IEnumerable<NppesResultDto>> SearchNppesApiAsync(Dictionary<string, string> parameters)
    {
        // Build the query string, ensuring we only include non-empty parameters
        var queryParams = parameters
            .Where(p => !string.IsNullOrEmpty(p.Value))
            .Select(p => $"{p.Key}={p.Value}");

        var fullQuery = $"?version=2.1&{string.Join("&", queryParams)}";

        try
        {
            var response = await _httpClient.GetAsync(fullQuery);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("NPPES API request failed with status code: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<NppesResultDto>();
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<NppesApiResponse>();
            return apiResponse?.Results ?? Enumerable.Empty<NppesResultDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "An error occurred while calling the NPPES API.");
            return Enumerable.Empty<NppesResultDto>();
        }
    }
}