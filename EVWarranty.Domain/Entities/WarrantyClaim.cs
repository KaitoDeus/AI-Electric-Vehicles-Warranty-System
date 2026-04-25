using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class WarrantyClaim
{
    public int ClaimId { get; set; }

    public string? Vin { get; set; }

    public int? Scid { get; set; }

    public int? CreatedBy { get; set; }

    public string? Description { get; set; }

    public string? DiagnosticNotes { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public int? ApprovedBy { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual ICollection<ClaimAttachment> ClaimAttachments { get; set; } = new List<ClaimAttachment>();

    public virtual ICollection<ClaimDetail> ClaimDetails { get; set; } = new List<ClaimDetail>();

    public virtual ICollection<ClaimStatusHistory> ClaimStatusHistories { get; set; } = new List<ClaimStatusHistory>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ServiceCenter? Sc { get; set; }

    public virtual Vehicle? VinNavigation { get; set; }
}
