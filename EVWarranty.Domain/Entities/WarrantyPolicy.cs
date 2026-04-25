using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class WarrantyPolicy
{
    public int PolicyId { get; set; }

    public int? PartId { get; set; }

    public string? ModelName { get; set; }

    public int? DurationMonths { get; set; }

    public int? MileageLimit { get; set; }

    public string? Conditions { get; set; }

    public virtual Part? Part { get; set; }
}
