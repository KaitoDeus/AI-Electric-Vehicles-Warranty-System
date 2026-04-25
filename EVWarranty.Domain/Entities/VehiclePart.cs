using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class VehiclePart
{
    public int VehiclePartId { get; set; }

    public string? Vin { get; set; }

    public int? PartId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public DateOnly? InstalledDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<ClaimDetail> ClaimDetails { get; set; } = new List<ClaimDetail>();

    public virtual Part? Part { get; set; }

    public virtual Vehicle? VinNavigation { get; set; }
}
