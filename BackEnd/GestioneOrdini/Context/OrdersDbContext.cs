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
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }

        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            

            // Configurazione per CustomerPrivate
            modelBuilder.Entity<CustomerPrivate>()
                .Property(cp => cp.CF)
                .HasMaxLength(16);

            // Configurazione per CustomerCompany
            modelBuilder.Entity<CustomerCompany>()
                .Property(cc => cc.PartitaIVA)
                .HasMaxLength(11);

            modelBuilder.Entity<CustomerCompany>()
                .Property(cc => cc.RagioneSociale)
                .HasMaxLength(255);

            // Configura la relazione tra Customer e Order
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configura la relazione tra User e Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            // Configura la relazione tra Item e WorkType
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Type)
                .WithMany()
                .HasForeignKey(i => i.WorkTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configura la proprietà per FileName e FilePath su Item
            modelBuilder.Entity<Item>()
                .Property(i => i.FileName)
                .HasMaxLength(255);

            modelBuilder.Entity<Item>()
                .Property(i => i.FilePath)
                .HasMaxLength(255);

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
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .OnDelete(DeleteBehavior.Restrict);

            // Configura il discriminatore per WorkType
            modelBuilder.Entity<Item>()
                .HasDiscriminator<int>("WorkTypeId")
                .HasValue<LaserItem>(1)
                .HasValue<PlotterItem>(2);

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

            // Configura la relazione tra Order e TotalAmount
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            // Precisione per LaserPriceList
            modelBuilder.Entity<LaserPriceList>()
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal(18,2)");

            // Configura la relazione tra Order e OrderStatus
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Status)
                .WithMany()
                .HasForeignKey(o => o.StatusId);
        }
    }
}
