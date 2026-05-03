using System;
using System.Collections.Generic;

namespace EVWarranty.Domain.Entities;

public partial class OtpToken
{
    public int OtpId { get; set; }

    public int? UserId { get; set; }

    public string OtpCode { get; set; } = null!;

    public DateTime ExpiryTime { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
