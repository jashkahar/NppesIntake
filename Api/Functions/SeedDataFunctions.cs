using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NppesIntake.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System;

namespace NppesIntake.Api.Functions;

public class SeedDataFunctions
{
    private readonly INpiIngestionService _ingestionService;
    private readonly ILogger<SeedDataFunctions> _logger;

    public SeedDataFunctions(INpiIngestionService ingestionService, ILogger<SeedDataFunctions> logger)
    {
        _ingestionService = ingestionService;
        _logger = logger;
    }

    // Renamed the function for clarity
    [Function("SeedAffiliationData")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "seed/affiliations")] HttpRequestData req)
    {
        _logger.LogInformation("Full affiliation seeding process started.");

        // Your predefined list of reliable NPI pairs
        var affiliationPairs = new List<(long prescriberNpi, long organizationNpi)>
        {
            (1679828693, 1790455590),
            (1851688675, 1427829522),
            (1851688675, 1477324291), // Note: This prescriber is linked to two orgs
            (1982026761, 1093137432),
            (1063899870, 1831585025)
        };

        int successCount = 0;
        foreach (var pair in affiliationPairs)
        {
            try
            {
                await _ingestionService.IngestAffiliationAsync(pair.prescriberNpi, pair.organizationNpi);
                _logger.LogInformation("Successfully seeded affiliation for Prescriber {prescriberNpi} and Org {organizationNpi}", pair.prescriberNpi, pair.organizationNpi);
                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to seed affiliation for Prescriber {prescriberNpi} and Org {organizationNpi}", pair.prescriberNpi, pair.organizationNpi);
            }
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync($"Seeding complete. {successCount} of {affiliationPairs.Count} affiliations were successfully processed.");
        return response;
    }
}