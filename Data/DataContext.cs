using LegendaryCruises.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LegendaryCruises.Data
{
    public class DataContext : IdentityDbContext
    {
        public DbSet<Cruise> Cruises { get; set; }

        public DbSet<ItineraryDay> ItineraryDays { get; set; }

        public DbSet<CruiseDate> CruiseDates { get; set; }

        public DbSet<DateCabin> DateCabins { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<PassengerInfo> PassengerInfos { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<QRCodeModel> QRCodeModels { get; set; }


        public DataContext(DbContextOptions options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // ==============================
            // Cruise - Itinerary
            // ==============================

            modelBuilder.Entity<ItineraryDay>()
                .HasOne(i => i.Cruise)
                .WithMany(c => c.Itinerary)
                .HasForeignKey(i => i.CruiseId)
                .OnDelete(DeleteBehavior.Cascade);



            // ==============================
            // CruiseDate - DateCabin
            // ==============================

            modelBuilder.Entity<CruiseDate>()
                .HasMany(d => d.Cabins)
                .WithOne(c => c.CruiseDate)
                .HasForeignKey(c => c.CruiseDateId)
                .OnDelete(DeleteBehavior.Cascade);



            // ==============================
            // CabinType enum jako string
            // ==============================

            modelBuilder.Entity<DateCabin>()
                .Property(c => c.CabinType)
                .HasConversion<string>();


            modelBuilder.Entity<CartItem>()
                .Property(c => c.CabinType)
                .HasConversion<string>();


            modelBuilder.Entity<OrderItem>()
                .Property(o => o.CabinType)
                .HasConversion<string>();



            // ==============================
            // Decimal precision
            // ==============================

            modelBuilder.Entity<DateCabin>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);


            modelBuilder.Entity<CartItem>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);


            modelBuilder.Entity<OrderItem>()
                .Property(o => o.Price)
                .HasPrecision(18, 2);


            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);



            // ==============================
            // Cart relations
            // ==============================

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cruise)
                .WithMany()
                .HasForeignKey(ci => ci.CruiseId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.CruiseDate)
                .WithMany()
                .HasForeignKey(ci => ci.CruiseDateId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.DateCabin)
                .WithMany()
                .HasForeignKey(ci => ci.DateCabinId)
                .OnDelete(DeleteBehavior.Restrict);



            // ==============================
            // Order relations
            // ==============================

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Cruise)
                .WithMany()
                .HasForeignKey(oi => oi.CruiseId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.CruiseDate)
                .WithMany()
                .HasForeignKey(oi => oi.CruiseDateId)
                .OnDelete(DeleteBehavior.Restrict);



            // NOWE:
            // OrderItem -> DateCabin

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.DateCabin)
                .WithMany()
                .HasForeignKey(oi => oi.DateCabinId)
                .OnDelete(DeleteBehavior.Restrict);



            // ==============================
            // QR Code
            // ==============================

            modelBuilder.Entity<QRCodeModel>()
                .HasOne(q => q.UserProfile)
                .WithMany(u => u.QRCodes)
                .HasForeignKey(q => q.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}