using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class Scinventory
{
    public int Scid { get; set; }

    public int PartId { get; set; }

    public int? Quantity { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Part Part { get; set; } = null!;

    public virtual ServiceCenter Sc { get; set; } = null!;
}
