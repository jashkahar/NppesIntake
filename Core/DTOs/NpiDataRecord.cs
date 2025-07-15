namespace NppesIntake.Core.DTOs;
public class NpiDataRecord
{
    public long Npi { get; set; }
    public string EnumerationType { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Credential { get; set; }
    public string? OrganizationName { get; set; }
    public Address? LocationAddress { get; set; }

    public class Address
    {
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
} 