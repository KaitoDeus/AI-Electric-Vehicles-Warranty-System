using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class ServiceCenter
{
    public int Scid { get; set; }

    public string Scname { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Scinventory> Scinventories { get; set; } = new List<Scinventory>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<WarrantyClaim> WarrantyClaims { get; set; } = new List<WarrantyClaim>();
}
