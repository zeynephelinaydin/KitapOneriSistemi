using Microsoft.EntityFrameworkCore;
using KitapOneriSistemi.Models;


namespace KitapOneriSistemi.Data
{
    public class ApplicationDbContext : DbContext
    {
        // DbSet'ler
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }

        // Constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // OnModelCreating ile Entity Configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User entity's table'ını 'public' şemasına eşliyoruz
            modelBuilder.Entity<User>()
                .ToTable("users", "public")
                .HasKey(u => u.UserId); // User tablosu için birincil anahtar UserId

            // Book entity's table'ını 'public' şemasına eşliyoruz
            modelBuilder.Entity<Book>()
                .ToTable("books", "public")
                .HasKey(b => b.ISBN); // Book tablosu için birincil anahtar

            // UserBook entity's table'ını 'public' şemasına eşliyoruz
            modelBuilder.Entity<UserBook>()
                .ToTable("user_books", "public")
                .HasKey(ub => new { ub.UserId, ub.ISBN }); // UserBook tablosu için birincil anahtar (UserId, ISBN)

            // UserBook tablosu için ilişkileri tanımlıyoruz
            modelBuilder.Entity<UserBook>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.UserBook) // UserBooks olmalı
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde, ilişkili UserBook kayıtları da silinsin

            modelBuilder.Entity<UserBook>()
                .HasOne(ub => ub.Book)
                .WithMany(b => b.UserBook) // UserBooks olmalı
                .HasForeignKey(ub => ub.ISBN)
                .OnDelete(DeleteBehavior.Cascade); // Kitap silindiğinde, ilişkili UserBook kayıtları da silinsin

            base.OnModelCreating(modelBuilder); // Temel yapılandırmaları eklemeyi unutmayın
        }
        
    }
    
    
}

