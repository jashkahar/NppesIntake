using NppesIntake.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NppesIntake.Core.Services;

public interface INppesApiService
{
    // The service now returns our clean, simple object
    Task<IEnumerable<NpiDataRecord>> SearchByNpiAsync(string npiNumber);
    Task<IEnumerable<NpiDataRecord>> SearchByNameAsync(string firstName, string lastName);
}