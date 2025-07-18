﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NppesIntake.Core.Entities;

public class BusinessUnit : AuditableEntity
{
    public long? Npi { get; set; }

    [Required]
    [MaxLength(255)]
    public string OrganizationName { get; set; }

    public ICollection<BusinessUnitMember> BusinessUnitMembers { get; set; } = new List<BusinessUnitMember>();
}