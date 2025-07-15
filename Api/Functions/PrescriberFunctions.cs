using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NppesIntake.Core.DTOs;
using NppesIntake.Core.Services;
using NppesIntake.Infrastructure;
using System.Net;

namespace NppesIntake.Api.Functions;
public class PrescriberFunctions
{
    private readonly INppesApiService _nppesService;
    private readonly INpiIngestionService _ingestionService;
    private readonly NppesIntakeDbContext _dbContext;
    private readonly ILogger<PrescriberFunctions> _logger;

    public PrescriberFunctions(INppesApiService nppesService, INpiIngestionService ingestionService, NppesIntakeDbContext dbContext, ILogger<PrescriberFunctions> logger)
    {
        _nppesService = nppesService;
        _ingestionService = ingestionService;
        _dbContext = dbContext;
        _logger = logger;
    }

    // FUNCTION 1: Lightweight Search
    [Function("SearchPrescribers")]
    public async Task<HttpResponseData> Search([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "prescribers/search")] HttpRequestData req)
    {
        string npi = req.Query["npi"];
        string firstName = req.Query["firstName"];
        string lastName = req.Query["lastName"];

        if (string.IsNullOrEmpty(npi) && string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("At least one search parameter (npi, firstName, lastName) is required.");
            return badRequest;
        }

        var externalResults = !string.IsNullOrEmpty(npi)
            ? await _nppesService.SearchByNpiAsync(npi)
            : await _nppesService.SearchByNameAsync(firstName, lastName);

        var searchResults = externalResults.Select(p => new PrescriberSearchResultDto
        {
            Npi = p.Npi,
            FirstName = p.FirstName ?? string.Empty,
            LastName = p.LastName ?? string.Empty
        });

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(searchResults);
        return response;
    }

    // FUNCTION 2: Internal Affiliation Lookup
    [Function("GetAffiliatedOrganizations")]
    public async Task<HttpResponseData> GetAffiliations([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "prescribers/{npi:long}/organizations")] HttpRequestData req, long npi)
    {
        var member = await _dbContext.Members
            .Include(m => m.BusinessUnitMembers)
            .ThenInclude(bum => bum.BusinessUnit)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Npi == npi);

        if (member == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var affiliatedOrgs = member.BusinessUnitMembers.Select(bum => new AffiliatedOrganizationDto
        {
            OrganizationId = bum.BusinessUnit.Id,
            OrganizationNpi = bum.BusinessUnit.Npi ?? 0,
            OrganizationName = bum.BusinessUnit.OrganizationName
        }).ToList();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(affiliatedOrgs);
        return response;
    }

    // FUNCTION 3: Transactional Ingest
    [Function("IngestAffiliation")]
    public async Task<HttpResponseData> Ingest([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ingest")] HttpRequestData req)
    {
        var ingestRequest = await req.ReadFromJsonAsync<IngestRequestDto>();
        if (ingestRequest == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid or missing request body.");
            return badRequest;
        }

        try
        {
            await _ingestionService.IngestAffiliationAsync(ingestRequest.PrescriberNpi, ingestRequest.OrganizationNpi);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Successfully ingested affiliation for Prescriber NPI {ingestRequest.PrescriberNpi} and Organization NPI {ingestRequest.OrganizationNpi}.");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ingestion failed for prescriber {pNpi} and org {oNpi}", ingestRequest.PrescriberNpi, ingestRequest.OrganizationNpi);
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}