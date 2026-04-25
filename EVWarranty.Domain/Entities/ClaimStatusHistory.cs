using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class ClaimStatusHistory
{
    public int HistoryId { get; set; }

    public int? ClaimId { get; set; }

    public string? FromStatus { get; set; }

    public string? ToStatus { get; set; }

    public int? ChangedBy { get; set; }

    public DateTime? ChangedAt { get; set; }

    public string? Comment { get; set; }

    public virtual User? ChangedByNavigation { get; set; }

    public virtual WarrantyClaim? Claim { get; set; }
}
