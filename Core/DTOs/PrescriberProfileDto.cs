namespace NppesIntake.Core.DTOs;

public class PrescriberProfileDto
{
    // Prescriber Info, taken from our clean NpiDataRecord
    public long Npi { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Context from our Database
    public bool IsAlreadyInDb { get; set; }
    public List<OrganizationDto> OrganizationsToLink { get; set; } = new();
}

public class OrganizationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
} 