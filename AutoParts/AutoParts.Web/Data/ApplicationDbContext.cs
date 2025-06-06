namespace AutoParts.Web.Data;

using AutoParts.Web.Data.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Part> Parts { get; set; }
    public DbSet<UsedPart> UsedParts { get; set; }
    public DbSet<ServiceTask> ServiceTasks { get; set; }
    public DbSet<ServiceOrder> ServiceOrders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<Vehicle>().ToTable("Vehicles");
        modelBuilder.Entity<Comment>().ToTable("Comments");
        modelBuilder.Entity<Part>().ToTable("Parts");
        modelBuilder.Entity<UsedPart>().ToTable("UsedParts");
        modelBuilder.Entity<ServiceTask>().ToTable("ServiceTasks");
        modelBuilder.Entity<ServiceOrder>().ToTable("ServiceOrders");
    }
}
