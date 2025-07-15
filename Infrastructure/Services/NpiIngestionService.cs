using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NppesIntake.Core.Entities;
using NppesIntake.Core.Services;

namespace NppesIntake.Infrastructure.Services;
public class NpiIngestionService : INpiIngestionService
{
    private readonly INppesApiService _nppesService;
    private readonly NppesIntakeDbContext _dbContext;
    private readonly ILogger<NpiIngestionService> _logger;

    public NpiIngestionService(INppesApiService nppesService, NppesIntakeDbContext dbContext, ILogger<NpiIngestionService> logger)
    {
        _nppesService = nppesService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task IngestAffiliationAsync(long prescriberNpi, long organizationNpi)
    {
        // 1. Fetch fresh data for BOTH entities
        var prescriberRecord = (await _nppesService.SearchByNpiAsync(prescriberNpi.ToString())).FirstOrDefault();
        var orgRecord = (await _nppesService.SearchByNpiAsync(organizationNpi.ToString())).FirstOrDefault();

        if (prescriberRecord is null || orgRecord is null || orgRecord.EnumerationType != "NPI-2")
        {
            throw new InvalidOperationException("Valid Prescriber and Organization NPIs must be provided.");
        }

        // 2. Upsert the Prescriber (Member)
        var member = await _dbContext.Members.Include(m => m.ContactDetails).FirstOrDefaultAsync(m => m.Npi == prescriberNpi);
        if (member == null)
        {
            member = new Member { MemberType = "Prescriber" };
            _dbContext.Members.Add(member);
        }
        else
        {
            _dbContext.Members.Update(member);
        }
        member.Npi = prescriberRecord.Npi;
        member.FirstName = prescriberRecord.FirstName;
        member.LastName = prescriberRecord.LastName;
        member.Credential = prescriberRecord.Credential;

        member.ContactDetails.Clear();
        if (prescriberRecord.LocationAddress != null)
        {
            member.ContactDetails.Add(new MemberContactDetails
            {
                AddressPurpose = "LOCATION",
                Address1 = prescriberRecord.LocationAddress.Address1,
                Address2 = prescriberRecord.LocationAddress.Address2,
                City = prescriberRecord.LocationAddress.City,
                State = prescriberRecord.LocationAddress.State,
                PostalCode = prescriberRecord.LocationAddress.PostalCode,
                PhoneNumber = prescriberRecord.LocationAddress.PhoneNumber
            });
        }

        // 3. Upsert the Organization (BusinessUnit)
        var businessUnit = await _dbContext.BusinessUnits.FirstOrDefaultAsync(b => b.Npi == organizationNpi);
        if (businessUnit == null)
        {
            businessUnit = new BusinessUnit();
            _dbContext.BusinessUnits.Add(businessUnit);
        }
        businessUnit.Npi = orgRecord.Npi;
        businessUnit.OrganizationName = orgRecord.OrganizationName;

        // 4. Create the link
        var relationshipExists = await _dbContext.BusinessUnitMembers
            .AnyAsync(bm => bm.MemberId == member.Id && bm.BusinessUnitId == businessUnit.Id);

        if (!relationshipExists)
        {
            _dbContext.BusinessUnitMembers.Add(new BusinessUnitMember { Member = member, BusinessUnit = businessUnit });
        }

        // 5. Save everything, which will also trigger the audit log
        await _dbContext.SaveChangesAsync();
    }
}