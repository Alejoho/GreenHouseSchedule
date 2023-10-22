using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SupportLayer.Models;

namespace DataAccess.Context;

public partial class SowScheduleDBEntities : DbContext
{
    public SowScheduleDBEntities()
    {
    }

    public SowScheduleDBEntities(DbContextOptions<SowScheduleDBEntities> options)
        : base(options)
    {
    }

    public virtual DbSet<Block> Blocks { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<DeliveryDetail> DeliveryDetails { get; set; }

    public virtual DbSet<GreenHouse> GreenHouses { get; set; }

    public virtual DbSet<Municipality> Municipalities { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderLocation> OrderLocations { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<SeedTray> SeedTrays { get; set; }

    public virtual DbSet<Species> Species { get; set; }

    public virtual DbSet<TypesOfOrganization> TypesOfOrganizations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;attachdbfilename=C:\\Users\\Alejo\\Documents\\GH\\GreenHouseSchedule\\DataAccess\\SowScheduleDB.mdf;integrated security=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Block>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.HasOne(d => d.OrderLocation).WithMany(p => p.Blocks)
                .HasForeignKey(d => d.OrderLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Blocks_OrderLocationId");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasIndex(e => e.Name, "UC_Clients_Name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.NickName).HasMaxLength(50);
            entity.Property(e => e.OtherNumber).HasMaxLength(20);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Organization).WithMany(p => p.Clients)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clients_OrganizationId");
        });

        modelBuilder.Entity<DeliveryDetail>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DeliveryDate).HasColumnType("date");

            entity.HasOne(d => d.Block).WithMany(p => p.DeliveryDetails)
                .HasForeignKey(d => d.BlockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryDetails_BlockId");
        });

        modelBuilder.Entity<GreenHouse>(entity =>
        {
            entity.HasIndex(e => e.Name, "UC_GreenHouses_Name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.GreenHouseArea).HasColumnType("numeric(5, 2)");
            entity.Property(e => e.Lenght).HasColumnType("numeric(4, 2)");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.SeedTrayArea).HasColumnType("numeric(5, 2)");
            entity.Property(e => e.Width).HasColumnType("numeric(4, 2)");
        });

        modelBuilder.Entity<Municipality>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Province).WithMany(p => p.Municipalities)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Municipalities_ProvinceId");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateOfRequest).HasColumnType("date");
            entity.Property(e => e.EstimateDeliveryDate).HasColumnType("date");
            entity.Property(e => e.EstimateSowDate).HasColumnType("date");
            entity.Property(e => e.RealDeliveryDate).HasColumnType("date");
            entity.Property(e => e.RealSowDate).HasColumnType("date");
            entity.Property(e => e.WishDate).HasColumnType("date");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_ClientId");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_ProductId");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.SeedsSource).HasMaxLength(50);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_OrderId");
        });

        modelBuilder.Entity<OrderLocation>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EstimateDeliveryDate).HasColumnType("date");
            entity.Property(e => e.RealDeliveryDate).HasColumnType("date");
            entity.Property(e => e.SowDate).HasColumnType("date");

            entity.HasOne(d => d.GreenHouse).WithMany(p => p.OrderLocations)
                .HasForeignKey(d => d.GreenHouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderLocations_GreenHouseId");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderLocations)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderLocations_OrderId");

            entity.HasOne(d => d.SeedTray).WithMany(p => p.OrderLocations)
                .HasForeignKey(d => d.SeedTrayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderLocations_SeedTrayId");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasIndex(e => e.Name, "UC_Organizations_Name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Municipalities).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.MunicipalitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Organizations_MunicipalityId");

            entity.HasOne(d => d.TypeOfOrganization).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.TypeOfOrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Organizations_TypesOfOrganizationId");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Variety).HasMaxLength(50);

            entity.HasOne(d => d.Specie).WithMany(p => p.Products)
                .HasForeignKey(d => d.SpecieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_SpecieId");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasIndex(e => e.Name, "UC_Provinces_Name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<SeedTray>(entity =>
        {
            entity.HasIndex(e => e.Name, "UC_SeedTrays_Name").IsUnique();

            entity.HasIndex(e => e.Preference, "UC_SeedTrays_Preference").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Material).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.TrayArea).HasColumnType("numeric(5, 4)");
            entity.Property(e => e.TrayLength).HasColumnType("numeric(3, 2)");
            entity.Property(e => e.TrayWidth).HasColumnType("numeric(3, 2)");
        });

        modelBuilder.Entity<Species>(entity =>
        {
            entity.HasIndex(e => e.Name, "UC_Species_Name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.WeightOf1000Seeds).HasColumnType("numeric(7, 3)");
            entity.Property(e => e.WeightOfSeedsPerHectare).HasColumnType("numeric(7, 3)");
        });

        modelBuilder.Entity<TypesOfOrganization>(entity =>
        {
            entity.ToTable("TypesOfOrganization");

            entity.HasIndex(e => e.Name, "UC_TypesOfOrganization_Name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
