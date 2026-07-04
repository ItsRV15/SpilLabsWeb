using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }

    public DbSet<Item> Items { get; set; }

    public DbSet<SalesOrder> SalesOrders { get; set; }

    public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Client -> SalesOrder
        modelBuilder.Entity<SalesOrder>()
            .HasOne(s => s.Client)
            .WithMany(c => c.SalesOrders)
            .HasForeignKey(s => s.ClientId);

        // SalesOrder -> SalesOrderDetail
        modelBuilder.Entity<SalesOrderDetail>()
            .HasOne(d => d.SalesOrder)
            .WithMany(o => o.SalesOrderDetails)
            .HasForeignKey(d => d.OrderId);

        // Item -> SalesOrderDetail
        modelBuilder.Entity<SalesOrderDetail>()
            .HasOne(d => d.Item)
            .WithMany(i => i.SalesOrderDetails)
            .HasForeignKey(d => d.ItemId);
    }
}