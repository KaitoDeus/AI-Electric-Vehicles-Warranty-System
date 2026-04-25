using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class ClaimAttachment
{
    public int AttachmentId { get; set; }

    public int? ClaimId { get; set; }

    public string? FileType { get; set; }

    public string? FilePath { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual WarrantyClaim? Claim { get; set; }
}
