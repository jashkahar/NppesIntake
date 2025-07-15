using NppesIntake.Core.Entities;
using System.Threading.Tasks;

namespace NppesIntake.Core.Services;

public interface INpiIngestionService
{
    Task IngestAffiliationAsync(long prescriberNpi, long organizationNpi);
}