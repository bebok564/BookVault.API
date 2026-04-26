using BookVault.API.Data;
using BookVault.API.DTO.Books;
using BookVault.API.DTO.Categories;
using BookVault.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BookVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CategoriesController : ControllerBase
    {
        private readonly BookVaultDbContext _context;

        public CategoriesController(BookVaultDbContext context)
        {
            _context = context;
        }

        //GET /api/categories — lista wszystkich kategorii
        [HttpGet]

        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _context.Categories
                                      .Select(c => new { c.Id, c.Name })
                                      .ToListAsync();
            return Ok(categories);
        }
        //POST /api/categories — dodaj nową
        [HttpPost]

        public async Task<ActionResult> AddCategory([FromBody] CreateCategoryDto dto)
        {
            var exists = await _context.Categories
                                        .AnyAsync(c => c.Name == dto.Name);
            if (exists)
            {
                return Conflict("Category already exists");
            }

            var category = new Category
            {
                Name = dto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategories),
                new { id = category.Id },
                new { category.Id, category.Name }
            );
        }
        //GET /api/categories/{id}/books — książki w danej kategorii
        [HttpGet]
        [Route("categories/{id}/books")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByCategory(int id)
        {
            
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound("Category not found");
                }

                var books = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Categories)
                    .Where(b => b.Categories.Any(c => c.Id == id))
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

                return Ok(books);
           


        }
    }
}             
