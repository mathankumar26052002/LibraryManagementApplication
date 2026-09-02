using LibraryManagementWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementWebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .HasIndex(x => new
                {
                    x.Title,
                    x.Author
                })
                .IsUnique();


            modelBuilder.Entity<BorrowRecord>()
                .HasOne(x => x.User)
                .WithMany(x => x.BorrowRecords)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<BorrowRecord>()
                .HasOne(x => x.Book)
                .WithMany(x => x.BorrowRecords)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}