using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NppesIntake.Core.DTOs;
using NppesIntake.Core.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NppesIntake.Api.Functions;

public class PrescriberFunctions
{
    private readonly ILogger<PrescriberFunctions> _logger;
    private readonly INppesApiService _nppesService;
    private readonly IPrescriberIntakeService _intakeService;

    public PrescriberFunctions(
        INppesApiService nppesService,
        IPrescriberIntakeService intakeService,
        ILogger<PrescriberFunctions> logger)
    {
        _nppesService = nppesService;
        _intakeService = intakeService;
        _logger = logger;
    }

    [Function("SearchPrescribers")]
    public async Task<HttpResponseData> Search(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "prescribers/search")] HttpRequestData req)
    {
        _logger.LogInformation("Prescriber search request received.");

        string npi = req.Query["npi"];
        string firstName = req.Query["firstName"];
        string lastName = req.Query["lastName"];

        // Check if at least one valid search parameter is provided
        if (string.IsNullOrEmpty(npi) && string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Please provide at least one search parameter: 'npi', 'firstName', or 'lastName'.");
            return badRequestResponse;
        }

        IEnumerable<NppesResultDto> fullResults;
        if (!string.IsNullOrEmpty(npi))
        {
            fullResults = await _nppesService.SearchByNpiAsync(npi);
        }
        else
        {
            fullResults = await _nppesService.SearchByNameAsync(firstName, lastName);
        }

        var simplifiedResults = fullResults.Select(p => new NppesSearchResultDto
        {
            Npi = p.Number,
            FirstName = p.Basic.FirstName,
            LastName = p.Basic.LastName
        });

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(simplifiedResults);
        return response;
    }

    [Function("IngestPrescriber")]
    public async Task<HttpResponseData> Ingest(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "prescribers/ingest")] HttpRequestData req)
    {
        _logger.LogInformation("Prescriber ingest request received.");

        var requestBody = await req.ReadFromJsonAsync<IngestRequest>();
        if (requestBody?.Npi == null || requestBody.Npi <= 0)
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Request body must include a valid 'npi' number.");
            return badRequestResponse;
        }

        var member = await _intakeService.IngestPrescriberByNpiAsync(requestBody.Npi);

        if (member == null)
        {
            _logger.LogWarning("Could not find or ingest prescriber for NPI: {Npi}", requestBody.Npi);
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { memberId = member.MemberId, npi = member.Npi, status = "Successfully Ingested" });
        return response;
    }
    public class IngestRequest
    {
        public long Npi { get; set; }
    }
}