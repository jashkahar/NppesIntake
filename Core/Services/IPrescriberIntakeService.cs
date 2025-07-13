using NppesIntake.Core.Entities;
using System.Threading.Tasks;

namespace NppesIntake.Core.Services;

public interface IPrescriberIntakeService
{
    Task<Member> IngestPrescriberByNpiAsync(long npi);
}