using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Models.Entities;

namespace MyFirstAPI.Data
{
    public class AplikasiDbContext : DbContext
    {
        public AplikasiDbContext(DbContextOptions<AplikasiDbContext> options) : base(options) { }
        public DbSet<Produk> Produk { get; set; }
        public DbSet<Kategori> Kategori { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionItem> TransactionItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Data awal kategori
            modelBuilder.Entity<Kategori>().HasData
                (
                new Kategori { Id = 1, Nama = "Elektronik" },
                new Kategori { Id = 2, Nama = "Pakaian" }
                );

            modelBuilder.Entity<Produk>()
        .Property(p => p.Harga)
        .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.Items)
                .WithOne(i => i.Transaction)
                .HasForeignKey(i => i.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
