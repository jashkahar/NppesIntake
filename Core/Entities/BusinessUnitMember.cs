using Microsoft.EntityFrameworkCore;

namespace NppesIntake.Core.Entities;

[PrimaryKey(nameof(MemberId), nameof(BusinessUnitId))]
public class BusinessUnitMember
{
    public int MemberId { get; set; }
    public Member Member { get; set; }

    public int BusinessUnitId { get; set; }
    public BusinessUnit BusinessUnit { get; set; }
}