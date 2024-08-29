using GestioneOrdini.Model.Clients;
using GestioneOrdini.Model.Order;
using GestioneOrdini.Model.PriceList;
using GestioneOrdini.Model.User;
using Microsoft.EntityFrameworkCore;

namespace GestioneOrdini.Context
{
    public class OrdersDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<LaserItem> LaserItems { get; set; }
        public DbSet<PlotterItem> PlotterItems { get; set; }
        public DbSet<LaserStandard> LaserStandards { get; set; }
        public DbSet<PlotterStandard> PlotterStandards { get; set; }
        public DbSet<LaserPriceList> LaserPriceLists { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; } // Aggiunta la configurazione per OrderStatus

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

            // Configura la relazione tra User e Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            // Configurazione delle entità Item per l'ereditarietà
            modelBuilder.Entity<Item>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<LaserItem>("Laser")
                .HasValue<PlotterItem>("Plotter");

            // Configurazione per LaserItem
            modelBuilder.Entity<LaserItem>()
                .HasOne(l => l.LaserStandard)
                .WithMany()
                .HasForeignKey(l => l.LaserStandardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurazione per PlotterItem
            modelBuilder.Entity<PlotterItem>()
                .HasOne(p => p.PlotterStandard)
                .WithMany()
                .HasForeignKey(p => p.PlotterStandardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurazione della relazione tra Order e Item
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Item)
                .WithMany()
                .HasForeignKey(o => o.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configura la relazione tra Order e OrderStatus
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Status)
                .WithMany()
                .HasForeignKey(o => o.StatusId);

            // Configurazione delle precisioni per i campi Price
            modelBuilder.Entity<LaserStandard>()
                .Property(ls => ls.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PlotterStandard>()
                .Property(ps => ps.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PlotterItem>()
                .Property(pi => pi.PricePerSquareMeter)
                .HasColumnType("decimal(18,2)");

            // Configura la relazione tra LaserItem e LaserPriceList
            modelBuilder.Entity<LaserItem>()
                .HasMany(li => li.LaserPriceLists)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
