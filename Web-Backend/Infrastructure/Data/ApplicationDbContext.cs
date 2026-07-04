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

        modelBuilder.Entity<SalesOrder>()
            .HasOne(x => x.Client)
            .WithMany(x => x.SalesOrders)
            .HasForeignKey(x => x.ClientId);

        modelBuilder.Entity<SalesOrderDetail>()
            .HasOne(x => x.SalesOrder)
            .WithMany(x => x.SalesOrderDetails)
            .HasForeignKey(x => x.OrderId);

        modelBuilder.Entity<SalesOrderDetail>()
            .HasOne(x => x.Item)
            .WithMany(x => x.SalesOrderDetails)
            .HasForeignKey(x => x.ItemId);
    }
}