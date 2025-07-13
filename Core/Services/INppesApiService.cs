using NppesIntake.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NppesIntake.Core.Services;

public interface INppesApiService
{
    Task<IEnumerable<NppesResultDto>> SearchByNpiAsync(string npiNumber);

    Task<IEnumerable<NppesResultDto>> SearchByNameAsync(string firstName, string lastName);
}