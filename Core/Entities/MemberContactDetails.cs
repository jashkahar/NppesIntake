using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NppesIntake.Core.Entities;

public class MemberContactDetails
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MemberId { get; set; }

    [ForeignKey("MemberId")]
    public Member Member { get; set; }

    [MaxLength(255)]
    public string? AddressPurpose { get; set; }

    [MaxLength(255)]
    public string? Address1 { get; set; }

    [MaxLength(255)]
    public string? Address2 { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
}