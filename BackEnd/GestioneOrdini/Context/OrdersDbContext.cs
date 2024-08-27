using GestioneOrdini.Model.Clients;
using GestioneOrdini.Model.Order;
using GestioneOrdini.Model.User;
using Microsoft.EntityFrameworkCore;

namespace GestioneOrdini.Context
{
    public class OrdersDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public DbSet<Dimension> Dimensions { get; set; }
        public DbSet<Role> Roles { get; set; }

        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura la gerarchia delle classi Customer
            modelBuilder.Entity<Customer>()
                .HasDiscriminator<string>("CustomerType")
                .HasValue<Customer>("Base")
                .HasValue<CustomerPrivate>("Private")
                .HasValue<CustomerCompany>("Company");

            // Configura la relazione tra Order e Dimension
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Dimension)
                .WithMany()
                .HasForeignKey(o => o.DimensionId);

            // Configura la relazione tra User e Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);
        }
    }
}
