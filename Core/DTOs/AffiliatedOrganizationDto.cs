namespace NppesIntake.Core.DTOs;
public class AffiliatedOrganizationDto
{
    public int OrganizationId { get; set; } // Internal DB Id
    public long OrganizationNpi { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
} 