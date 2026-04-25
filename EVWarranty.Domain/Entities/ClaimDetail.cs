using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class ClaimDetail
{
    public int DetailId { get; set; }

    public int? ClaimId { get; set; }

    public int? VehiclePartId { get; set; }

    public string? ActionType { get; set; }

    public string? NewSerialNumber { get; set; }

    public string? Note { get; set; }

    public virtual WarrantyClaim? Claim { get; set; }

    public virtual VehiclePart? VehiclePart { get; set; }
}
