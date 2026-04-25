using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public int? RoleId { get; set; }

    public int? Scid { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<ClaimStatusHistory> ClaimStatusHistories { get; set; } = new List<ClaimStatusHistory>();

    public virtual Role? Role { get; set; }

    public virtual ServiceCenter? Sc { get; set; }

    public virtual ICollection<WarrantyClaim> WarrantyClaimApprovedByNavigations { get; set; } = new List<WarrantyClaim>();

    public virtual ICollection<WarrantyClaim> WarrantyClaimCreatedByNavigations { get; set; } = new List<WarrantyClaim>();
}
