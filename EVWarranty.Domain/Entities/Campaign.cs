using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class Campaign
{
    public int CampaignId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<CampaignVehicle> CampaignVehicles { get; set; } = new List<CampaignVehicle>();
}
