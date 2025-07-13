using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NppesIntake.Core.Entities;

public class Member
{
    [Key]
    public int MemberId { get; set; }

    [Required]
    public long Npi { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }

    [MaxLength(50)]
    public string? Credential { get; set; }

    [Required]
    [MaxLength(50)]
    public string MemberType { get; set; }

    public ICollection<MemberContactDetails> ContactDetails { get; set; } = new List<MemberContactDetails>();

    public ICollection<BusinessUnitMember> BusinessUnitMembers { get; set; } = new List<BusinessUnitMember>();
}