using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderDetail> SalesOrderDetails => Set<SalesOrderDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("client");
            entity.HasKey(e => e.ClientId);
            entity.Property(e => e.ClientId).HasColumnName("ClientId").ValueGeneratedOnAdd();
            entity.Property(e => e.CustomerName).HasColumnName("CustomerName").IsRequired();
            entity.Property(e => e.Address1).HasColumnName("Address1");
            entity.Property(e => e.Address2).HasColumnName("Address2");
            entity.Property(e => e.Address3).HasColumnName("Address3");
            entity.Property(e => e.Suburb).HasColumnName("Suburb");
            entity.Property(e => e.State).HasColumnName("State");
            entity.Property(e => e.PostCode).HasColumnName("PostCode");
            entity.Property(e => e.Phone).HasColumnName("Phone");
            entity.Property(e => e.Email).HasColumnName("Email");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("item");
            entity.HasKey(e => e.ItemId);
            entity.Property(e => e.ItemId).HasColumnName("ItemId").ValueGeneratedOnAdd();
            entity.Property(e => e.ItemCode).HasColumnName("ItemCode").IsRequired();
            entity.Property(e => e.Description).HasColumnName("Description").IsRequired();
            entity.Property(e => e.Price).HasColumnName("Price");
        });

        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.ToTable("salesorder");
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderId).HasColumnName("OrderId").ValueGeneratedOnAdd();
            entity.Property(e => e.ClientId).HasColumnName("ClientId");
            entity.Property(e => e.InvoiceNo).HasColumnName("InvoiceNo").IsRequired();
            entity.Property(e => e.InvoiceDate).HasColumnName("InvoiceDate");
            entity.Property(e => e.ReferenceNo).HasColumnName("ReferenceNo");
            entity.Property(e => e.Note).HasColumnName("Note");
            entity.Property(e => e.TotalExcl).HasColumnName("TotalExcl");
            entity.Property(e => e.TotalTax).HasColumnName("TotalTax");
            entity.Property(e => e.TotalIncl).HasColumnName("TotalIncl");

            entity.HasOne(x => x.Client)
                .WithMany(x => x.SalesOrders)
                .HasForeignKey(x => x.ClientId);
        });

        modelBuilder.Entity<SalesOrderDetail>(entity =>
        {
            entity.ToTable("salesorderdetail");
            entity.HasKey(e => e.DetailId);
            entity.Property(e => e.DetailId).HasColumnName("DetailId").ValueGeneratedOnAdd();
            entity.Property(e => e.OrderId).HasColumnName("OrderId");
            entity.Property(e => e.ItemId).HasColumnName("ItemId");
            entity.Property(e => e.Note).HasColumnName("Note");
            entity.Property(e => e.Quantity).HasColumnName("Quantity");
            entity.Property(e => e.Price).HasColumnName("Price");
            entity.Property(e => e.TaxRate).HasColumnName("TaxRate");
            entity.Property(e => e.ExclAmount).HasColumnName("ExclAmount");
            entity.Property(e => e.TaxAmount).HasColumnName("TaxAmount");
            entity.Property(e => e.InclAmount).HasColumnName("InclAmount");

            entity.HasOne(x => x.SalesOrder)
                .WithMany(x => x.SalesOrderDetails)
                .HasForeignKey(x => x.OrderId);

            entity.HasOne(x => x.Item)
                .WithMany(x => x.SalesOrderDetails)
                .HasForeignKey(x => x.ItemId);
        });
    }
}