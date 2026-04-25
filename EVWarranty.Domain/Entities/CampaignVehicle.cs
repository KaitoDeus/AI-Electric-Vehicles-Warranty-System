using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class CampaignVehicle
{
    public int CampaignId { get; set; }

    public string Vin { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual Vehicle VinNavigation { get; set; } = null!;
}
