using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EVWarranty.Domain.Entities;

namespace EVWarranty.Infrastructure.Data;

public partial class EvWarrantyDbContext : DbContext
{
    public EvWarrantyDbContext(DbContextOptions<EvWarrantyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<CampaignVehicle> CampaignVehicles { get; set; }

    public virtual DbSet<ClaimAttachment> ClaimAttachments { get; set; }

    public virtual DbSet<ClaimDetail> ClaimDetails { get; set; }

    public virtual DbSet<ClaimStatusHistory> ClaimStatusHistories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Part> Parts { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Scinventory> Scinventories { get; set; }

    public virtual DbSet<ServiceCenter> ServiceCenters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehiclePart> VehicleParts { get; set; }

    public virtual DbSet<WarrantyClaim> WarrantyClaims { get; set; }

    public virtual DbSet<WarrantyPolicy> WarrantyPolicies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.CampaignId).HasName("PK__Campaign__3F5E8D79D0281BF4");

            entity.Property(e => e.CampaignId).HasColumnName("CampaignID");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasDefaultValue("Recall");
        });

        modelBuilder.Entity<CampaignVehicle>(entity =>
        {
            entity.HasKey(e => new { e.CampaignId, e.Vin }).HasName("PK__Campaign__F3037F4DFCAB49CD");

            entity.Property(e => e.CampaignId).HasColumnName("CampaignID");
            entity.Property(e => e.Vin)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("VIN");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Not Started");

            entity.HasOne(d => d.Campaign).WithMany(p => p.CampaignVehicles)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CampaignV__Campa__30C33EC3");

            entity.HasOne(d => d.VinNavigation).WithMany(p => p.CampaignVehicles)
                .HasForeignKey(d => d.Vin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CampaignVeh__VIN__31B762FC");
        });

        modelBuilder.Entity<ClaimAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__ClaimAtt__442C64DEF2C02440");

            entity.Property(e => e.AttachmentId).HasColumnName("AttachmentID");
            entity.Property(e => e.ClaimId).HasColumnName("ClaimID");
            entity.Property(e => e.FilePath).HasMaxLength(1000);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Claim).WithMany(p => p.ClaimAttachments)
                .HasForeignKey(d => d.ClaimId)
                .HasConstraintName("FK__ClaimAtta__Claim__25518C17");
        });

        modelBuilder.Entity<ClaimDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__ClaimDet__135C314D0013FD3C");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.ClaimId).HasColumnName("ClaimID");
            entity.Property(e => e.NewSerialNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.VehiclePartId).HasColumnName("VehiclePartID");

            entity.HasOne(d => d.Claim).WithMany(p => p.ClaimDetails)
                .HasForeignKey(d => d.ClaimId)
                .HasConstraintName("FK__ClaimDeta__Claim__2180FB33");

            entity.HasOne(d => d.VehiclePart).WithMany(p => p.ClaimDetails)
                .HasForeignKey(d => d.VehiclePartId)
                .HasConstraintName("FK__ClaimDeta__Vehic__22751F6C");
        });

        modelBuilder.Entity<ClaimStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__ClaimSta__4D7B4ADDBC113EAC");

            entity.ToTable("ClaimStatusHistory");

            entity.Property(e => e.HistoryId).HasColumnName("HistoryID");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ClaimId).HasColumnName("ClaimID");
            entity.Property(e => e.FromStatus).HasMaxLength(50);
            entity.Property(e => e.ToStatus).HasMaxLength(50);

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.ClaimStatusHistories)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("FK__ClaimStat__Chang__2A164134");

            entity.HasOne(d => d.Claim).WithMany(p => p.ClaimStatusHistories)
                .HasForeignKey(d => d.ClaimId)
                .HasConstraintName("FK__ClaimStat__Claim__29221CFB");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B860C76E7A");

            entity.HasIndex(e => e.Idnumber, "UQ__Customer__564DB08A678AAA78").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__Customer__5C7E359ECDA2F135").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Idnumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDNumber");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.PartId).HasName("PK__Parts__7C3F0D30AC18BF25");

            entity.HasIndex(e => e.PartCode, "UQ__Parts__6525D39D86980A30").IsUnique();

            entity.Property(e => e.PartId).HasColumnName("PartID");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.PartCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PartName).HasMaxLength(200);
            entity.Property(e => e.Unit).HasMaxLength(20);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A1377063E");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160923CC0E1").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Scinventory>(entity =>
        {
            entity.HasKey(e => new { e.Scid, e.PartId }).HasName("PK__SCInvent__E03D637F3CB44120");

            entity.ToTable("SCInventory");

            entity.Property(e => e.Scid).HasColumnName("SCID");
            entity.Property(e => e.PartId).HasColumnName("PartID");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasDefaultValue(0);

            entity.HasOne(d => d.Part).WithMany(p => p.Scinventories)
                .HasForeignKey(d => d.PartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SCInvento__PartI__0B91BA14");

            entity.HasOne(d => d.Sc).WithMany(p => p.Scinventories)
                .HasForeignKey(d => d.Scid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SCInventor__SCID__0A9D95DB");
        });

        modelBuilder.Entity<ServiceCenter>(entity =>
        {
            entity.HasKey(e => e.Scid).HasName("PK__ServiceC__F7FE93ACEE696791");

            entity.Property(e => e.Scid).HasColumnName("SCID");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Scname)
                .HasMaxLength(200)
                .HasColumnName("SCName");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC2F9958B4");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4137B706F").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Scid).HasColumnName("SCID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__RoleID__7A672E12");

            entity.HasOne(d => d.Sc).WithMany(p => p.Users)
                .HasForeignKey(d => d.Scid)
                .HasConstraintName("FK__Users__SCID__7B5B524B");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Vin).HasName("PK__Vehicles__C5DF234D10B63F6E");

            entity.Property(e => e.Vin)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("VIN");
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.CurrentMileage).HasDefaultValue(0);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.ModelName).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Customer).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Vehicles__Custom__02FC7413");
        });

        modelBuilder.Entity<VehiclePart>(entity =>
        {
            entity.HasKey(e => e.VehiclePartId).HasName("PK__VehicleP__37AA60B2D3FF494D");

            entity.HasIndex(e => e.SerialNumber, "UQ__VehicleP__048A00088886EC5C").IsUnique();

            entity.Property(e => e.VehiclePartId).HasColumnName("VehiclePartID");
            entity.Property(e => e.InstalledDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PartId).HasColumnName("PartID");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Normal");
            entity.Property(e => e.Vin)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("VIN");

            entity.HasOne(d => d.Part).WithMany(p => p.VehicleParts)
                .HasForeignKey(d => d.PartId)
                .HasConstraintName("FK__VehiclePa__PartI__123EB7A3");

            entity.HasOne(d => d.VinNavigation).WithMany(p => p.VehicleParts)
                .HasForeignKey(d => d.Vin)
                .HasConstraintName("FK__VehiclePart__VIN__114A936A");
        });

        modelBuilder.Entity<WarrantyClaim>(entity =>
        {
            entity.HasKey(e => e.ClaimId).HasName("PK__Warranty__EF2E13BB3AFE439D");

            entity.Property(e => e.ClaimId).HasColumnName("ClaimID");
            entity.Property(e => e.ApprovedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Scid).HasColumnName("SCID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Vin)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("VIN");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.WarrantyClaimApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__WarrantyC__Appro__1EA48E88");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WarrantyClaimCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__WarrantyC__Creat__1BC821DD");

            entity.HasOne(d => d.Sc).WithMany(p => p.WarrantyClaims)
                .HasForeignKey(d => d.Scid)
                .HasConstraintName("FK__WarrantyCl__SCID__1AD3FDA4");

            entity.HasOne(d => d.VinNavigation).WithMany(p => p.WarrantyClaims)
                .HasForeignKey(d => d.Vin)
                .HasConstraintName("FK__WarrantyCla__VIN__19DFD96B");
        });

        modelBuilder.Entity<WarrantyPolicy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PK__Warranty__2E133944EBF711E7");

            entity.Property(e => e.PolicyId).HasColumnName("PolicyID");
            entity.Property(e => e.ModelName).HasMaxLength(100);
            entity.Property(e => e.PartId).HasColumnName("PartID");

            entity.HasOne(d => d.Part).WithMany(p => p.WarrantyPolicies)
                .HasForeignKey(d => d.PartId)
                .HasConstraintName("FK__WarrantyP__PartI__17036CC0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
