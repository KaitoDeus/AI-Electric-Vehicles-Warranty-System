using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class Part
{
    public int PartId { get; set; }

    public string? PartCode { get; set; }

    public string PartName { get; set; } = null!;

    public string? Category { get; set; }

    public string? Unit { get; set; }

    public virtual ICollection<Scinventory> Scinventories { get; set; } = new List<Scinventory>();

    public virtual ICollection<VehiclePart> VehicleParts { get; set; } = new List<VehiclePart>();

    public virtual ICollection<WarrantyPolicy> WarrantyPolicies { get; set; } = new List<WarrantyPolicy>();
}
