using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NppesIntake.Core.Entities;
using NppesIntake.Core.Services;
using System.Linq;
using System.Threading.Tasks;

namespace NppesIntake.Infrastructure.Services;

public class PrescriberIntakeService : IPrescriberIntakeService
{
    private readonly INppesApiService _nppesService;
    private readonly NppesIntakeDbContext _dbContext;
    private readonly ILogger<PrescriberIntakeService> _logger;

    public PrescriberIntakeService(
        INppesApiService nppesService,
        NppesIntakeDbContext dbContext,
        ILogger<PrescriberIntakeService> logger)
    {
        _nppesService = nppesService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Member> IngestPrescriberByNpiAsync(long npi)
    {
        // 1. Get fresh data from the external API
        var apiData = (await _nppesService.SearchByNpiAsync(npi.ToString())).FirstOrDefault();
        if (apiData == null)
        {
            _logger.LogWarning("No prescriber found in NPPES registry for NPI: {Npi}", npi);
            return null;
        }

        // 2. Find existing member and their relationships, or create a new one
        var member = await _dbContext.Members
            .Include(m => m.ContactDetails)
            .Include(m => m.BusinessUnitMembers)
            .ThenInclude(bum => bum.BusinessUnit)
            .FirstOrDefaultAsync(m => m.Npi == npi);

        if (member == null)
        {
            member = new Member();
            _dbContext.Members.Add(member);
            _logger.LogInformation("Creating new member for NPI: {Npi}", npi);
        }
        else
        {
            _logger.LogInformation("Updating existing member for NPI: {Npi}", npi);
        }

        // 3. Update Member's basic information
        member.Npi = apiData.Number;
        member.FirstName = apiData.Basic.FirstName;
        member.LastName = apiData.Basic.LastName;
        member.MemberType = "Prescriber";
        member.Credential = apiData.Basic.Credential;

        // 4. Update Member's Contact Details (simple clear and re-add strategy)
        member.ContactDetails.Clear();
        
        var locationAddress = apiData.Addresses.FirstOrDefault(
            a => a.AddressPurpose == "LOCATION");
        
        if (locationAddress != null)
        {
            member.ContactDetails.Add(new MemberContactDetails
            {
                AddressPurpose = locationAddress.AddressPurpose,
                Address1 = locationAddress.Address1,
                Address2 = locationAddress.Address2,
                City = locationAddress.City,
                State = locationAddress.State,
                PostalCode = locationAddress.PostalCode,
                PhoneNumber = locationAddress.TelephoneNumber
            });
        }

        // 5. Handle the Organization and its relationship to the Member
        if (!string.IsNullOrEmpty(apiData.Basic.OrganizationName))
        {
            var orgName = apiData.Basic.OrganizationName;
            var businessUnit = await _dbContext.BusinessUnits.FirstOrDefaultAsync(b => b.OrganizationName == orgName);
            if (businessUnit == null)
            {
                businessUnit = new BusinessUnit { OrganizationName = orgName };
                _dbContext.BusinessUnits.Add(businessUnit);
                _logger.LogInformation("Creating new Business Unit: {OrganizationName}", orgName);
            }

            // Ensure the relationship exists
            var relationshipExists = member.BusinessUnitMembers.Any(bm => bm.BusinessUnit == businessUnit || bm.BusinessUnitId == businessUnit.BusinessUnitId);
            if (!relationshipExists)
            {
                member.BusinessUnitMembers.Add(new BusinessUnitMember { BusinessUnit = businessUnit });
                _logger.LogInformation("Linking member {Npi} to Business Unit {OrganizationName}", npi, orgName);
            }
        }

        try
        {
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully ingested data for NPI: {Npi}", npi);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("Cannot insert duplicate key") ?? false)
        {
            _logger.LogWarning(ex, "Race condition detected for NPI {Npi}. Another process inserted it first. Fetching existing record.", npi);
            return await _dbContext.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Npi == npi);
        }

        return member;
    }
}