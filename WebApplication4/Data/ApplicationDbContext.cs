using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Models;

namespace WebApplication4.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(b =>
            {
                b.HasMany(b => b.BookGenres) // Use the join entity
                 .WithOne(bg => bg.Book)
                 .HasForeignKey(bg => bg.BookId);
            });

            modelBuilder.Entity<Genre>(g =>
            {
                g.HasMany(g => g.BookGenres) // Use the join entity
                 .WithOne(bg => bg.Genre)
                 .HasForeignKey(bg => bg.GenreId);
            });

            modelBuilder.Entity<BookGenre>(bg =>
            {
                bg.HasKey(bg => new { bg.BookId, bg.GenreId }); // Composite key
            });
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public DbSet<BookGenre> BookGenres { get; set; }
    }
}