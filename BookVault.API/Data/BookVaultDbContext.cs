using BookVault.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookVault.API.Data
{
    public class BookVaultDbContext : DbContext
    {
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Review> Reviews => Set<Review>();

        public BookVaultDbContext(DbContextOptions<BookVaultDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ISBN musi być unikalny
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Isbn)
                .IsUnique();

            // Kaskadowe usuwanie recenzji z książką
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Book>()
                 .Property(b => b.AverageRating)
                 .HasPrecision(3, 2);
            modelBuilder.Entity<Book>()
                 .HasMany(b => b.Categories)
                 .WithMany(c => c.Books)
                 .UsingEntity(j => j.HasData(
    new { BooksId = 1, CategoriesId = 1 },  // Witcher → Fantasy
    new { BooksId = 1, CategoriesId = 3 },  // Witcher → Classic
    new { BooksId = 2, CategoriesId = 2 },  // Dune → Sci-Fi
    new { BooksId = 3, CategoriesId = 3 }   // 1984 → Classic
    ));

            // Seed data — możesz tu dodać testowe dane

            modelBuilder.Entity<Author>().HasData(
            new Author
            {
                Id = 1,
                FirstName = "Andrzej",
                LastName = "Sapkowski",
                Bio = "Polish fantasy writer",
                BirthDate = new DateOnly(1948, 6, 21)
            },
            new Author
            {
                Id = 2,
                FirstName = "Frank",
                LastName = "Herbert",
                Bio = "American science fiction author"
            },
            new Author
            {
                 Id = 3,
                 FirstName = "George",
                 LastName = "Orwell",
                 Bio = "English novelist and essayist"
            }
           );
            // Kategorie — min. 5 (np. Fantasy, Sci-Fi, Classic, Horror, Romance)
            modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Fantasy"
            },
            new Category
            {
                Id = 2,
                Name = "Science Fiction"
            },
            new Category
            {
                Id = 3,
                Name = "Classic"
            },
            new Category
            {
                Id = 4,
                Name = "Horror"
            },
            new Category
            {
                Id = 5,
                Name = "Romance"
            }
            );

            //Książki — min. 3 (pamiętaj o AuthorId i AverageRating = 0)
            modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Title = "The Witcher: The Last Wish",
                Isbn = "9780316029186",
                PageCount = 288,
                PublishedDate = new DateOnly(1993, 1, 1),
                AuthorId = 1,
                AverageRating = 0
            },
            new Book
            {
                Id = 2,
                Title = "Dune",
                Isbn = "9780441013593",
                PageCount = 412,
                PublishedDate = new DateOnly(1965, 8, 1),
                AuthorId = 2,
                AverageRating = 0
            },
new Book
{
    Id = 3,
    Title = "1984",
    Isbn = "9780451524935",
    PageCount = 328,
    PublishedDate = new DateOnly(1949, 6, 8),
    AuthorId = 3,
    AverageRating = 0
        }
        );


        }
    }
}

