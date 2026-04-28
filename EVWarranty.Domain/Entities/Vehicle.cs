using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class Vehicle
{
    public string Vin { get; set; } = null!;

    public string ModelName { get; set; } = null!;

    public string? Color { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public Guid? CustomerId { get; set; }

    public int? CurrentMileage { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<CampaignVehicle> CampaignVehicles { get; set; } = new List<CampaignVehicle>();

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<VehiclePart> VehicleParts { get; set; } = new List<VehiclePart>();

    public virtual ICollection<WarrantyClaim> WarrantyClaims { get; set; } = new List<WarrantyClaim>();
}
