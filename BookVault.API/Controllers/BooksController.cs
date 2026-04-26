using BookVault.API.Data;
using BookVault.API.DTO.Authors;
using BookVault.API.DTO.Books;
using BookVault.API.DTO.Reviews;
using BookVault.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BookVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookVaultDbContext _context;

        public BooksController(BookVaultDbContext context)
        {
            _context = context;
        }
        //GET /api/books
        [HttpGet]
        public async Task<ActionResult<PagedResult<BookDto>>> GetBooks(
     [FromQuery] BookQueryParameters query)
        {
            var booksQuery = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .AsQueryable();

            // Filtrowanie po tytule lub autorze
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                booksQuery = booksQuery.Where(b =>
                    b.Title.Contains(query.Search) ||
                    b.Author.FirstName.Contains(query.Search) ||
                    b.Author.LastName.Contains(query.Search));
            }

            // Filtrowanie po kategorii
            if (query.CategoryId.HasValue)
            {
                booksQuery = booksQuery.Where(b =>
                    b.Categories.Any(c => c.Id == query.CategoryId));
            }

            // Sortowanie
            booksQuery = query.SortBy?.ToLower() switch
            {
                "rating" => booksQuery.OrderByDescending(b => b.AverageRating),
                "date" => booksQuery.OrderByDescending(b => b.PublishedDate),
                "author" => booksQuery.OrderBy(b => b.Author.LastName),
                _ => booksQuery.OrderBy(b => b.Title)
            };

            // Łączna liczba PRZED paginacją
            var totalCount = await booksQuery.CountAsync();

            // Paginacja + mapowanie na DTO
            var books = await booksQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(b => new BookDto(
                    b.Id,
                    b.Title,
                    b.Isbn,
                    b.PageCount,
                    $"{b.Author.FirstName} {b.Author.LastName}",
                    b.AverageRating,
                    b.Categories.Select(c => c.Name).ToList()
                ))
                .ToListAsync();

            return Ok(new PagedResult<BookDto>(
                books, totalCount, query.Page, query.PageSize));
        }

        //GET /api/books/{id}  zwracasz BookDetailDto z autorem, kategoriami i recenzjami.
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailDto>> GetBookDetails(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var dto = new BookDetailDto(
            book.Id,
            book.Title,
            book.Isbn,
            book.Description,
            book.PageCount,
            book.PublishedDate,
            book.AverageRating,
                new AuthorDto(
                 book.Author.Id,
                 book.Author.FirstName,
                 book.Author.LastName,
                 book.Author.Bio
           ),
              book.Categories.Select(c => c.Name).ToList(),
              book.Reviews.Select(r => new ReviewDto(
                r.Id,
                r.ReviewerName,
                r.Comment,
                r.Rating,
                r.CreatedAt
           )).ToList()
           );

            return Ok(dto);
        }

        // POST /api/books  relacja many-to-many z kategoriami
        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto dto)
        {
            // 1. Sprawdź czy autor istnieje
            var author = await _context.Authors.FindAsync(dto.AuthorId);
            if (author == null)
            {
                return BadRequest("Author not found");
            }

            // 2. Sprawdź unikalność ISBN
            var isbnExists = await _context.Books
                .AnyAsync(b => b.Isbn == dto.Isbn);
            if (isbnExists)
            {
                return Conflict("Book with this ISBN already exists");
            }

            // 3. Pobierz kategorie z bazy po ID
            var categories = await _context.Categories
                .Where(c => dto.CategoryIds.Contains(c.Id))
                .ToListAsync();

            if (categories.Count != dto.CategoryIds.Count)
            {
                return BadRequest("One or more categories not found");
            }

            // 4. Stwórz encję Book
            var book = new Book
            {
                Title = dto.Title,
                Isbn = dto.Isbn,
                Description = dto.Description,
                PageCount = dto.PageCount,
                PublishedDate = dto.PublishedDate,
                AuthorId = dto.AuthorId,
                AverageRating = 0,
                Categories = categories
            };

            // 5. Zapisz
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // 6. Zwróć 201 Created
            return CreatedAtAction(
          nameof(GetBookDetails),
          new { id = book.Id },
             new BookDto(
              book.Id,
              book.Title,
              book.Isbn,
              book.PageCount,
              $"{author.FirstName} {author.LastName}",
              book.AverageRating,
              categories.Select(c => c.Name).ToList()
             )
           );
        }

        // PUT /api/books/{id} — pamiętaj, że aktualizacja kategorii jest trudniejsza niż przy tworzeniu.
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBook (int id, [FromBody] UpdateBookDto dto)
            {
            // Pobierz książkę z bazy po id z Include na Categories (bo będziesz je aktualizować)
            var book = await _context.Books
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);
            // Sprawdź czy książka istnieje → jeśli nie, 404
            if (book == null)
            {
                return NotFound();
            }
            //Sprawdź czy autor o podanym AuthorId istnieje → jeśli nie, 400
            var authorExists = await _context.Authors
                .AnyAsync(a => a.Id == dto.AuthorId);
              if (!authorExists)
              {
                   return BadRequest("Author not found");
              }
            //Sprawdź czy nowy ISBN nie jest już zajęty przez inną książkę(nie przez tę samą! bo użytkownik może nie zmieniać ISBN)
            var isbnExists = await _context.Books
                .AnyAsync(b => b.Isbn == dto.Isbn && b.Id != id);

            if (isbnExists)
            {
                return Conflict("Book with this ISBN already exists");
            }
            //Pobierz nowe kategorie z bazy po CategoryIds
            var newCategories = await _context.Categories
                .Where(c => dto.CategoryIds.Contains(c.Id))
                .ToListAsync();
            //Sprawdź czy wszystkie kategorie istnieją(porównaj Count)
                if (newCategories.Count != dto.CategoryIds.Count)
                {
                    return BadRequest("One or more categories not found");
                }
              //Zaktualizuj właściwości książki(Title, Isbn, Description, PageCount, PublishedDate, AuthorId)
              book.Title = dto.Title;
              book.Isbn = dto.Isbn;
              book.Description = dto.Description;
              book.PageCount = dto.PageCount;
              book.PublishedDate = dto.PublishedDate;
              book.AuthorId = dto.AuthorId;
            //Wyczyść stare kategorie i przypisz nowe — book.Categories.Clear() potem book.Categories = newCategories
            book.Categories.Clear();
            foreach (var cat in newCategories)
                book.Categories.Add(cat);
            //SaveChangesAsync()
            await _context.SaveChangesAsync();
              //Zwróć NoContent() (204)
              return NoContent();
        }

        //DELETE /api/books/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook (int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        //GET /api/books/{id}/reviews — lista recenzji danej książki
        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<List<ReviewDto>>> GetBookReviews (int id)
        {
            var book = await _context.Books
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            var reviews = book.Reviews.Select(r => new ReviewDto
            (
                r.Id,
                r.ReviewerName,
                r.Comment,
                r.Rating,
                r.CreatedAt  
            )).ToList();

            return Ok(reviews);
        }
        //POST /api/books/{id}/reviews — dodaj recenzję i przelicz AverageRating
        [HttpPost("{id}/reviews")]
        public async Task<ActionResult<ReviewDto>> AddBookReview (int id, [FromBody] CreateReviewDto dto)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var review = new Review
            {
     
               ReviewerName = dto.ReviewerName,
               Rating = dto.Rating,
               Comment = dto.Comment,
               CreatedAt = DateTime.UtcNow,
               BookId = id
            };
            
            //pamiętaj o przeliczeniu AverageRating po dodaniu.

            book.Reviews.Add(review);
            book.AverageRating = (decimal)book.Reviews.Average(r => r.Rating);

            return CreatedAtAction(
            nameof(GetBookReviews),
            new { id = book.Id },
            new ReviewDto(
                review.Id,
                review.ReviewerName,
                review.Comment,
                review.Rating,
                review.CreatedAt
            )
            );


        }

    }
}
