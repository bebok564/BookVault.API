using BookVault.API.Data;
using BookVault.API.DTO.Authors;
using BookVault.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BookVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly BookVaultDbContext _context;

        public AuthorsController(BookVaultDbContext context)
        {
            _context = context;
        }

        //GET /api/authors — lista autorów
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            var authors = await _context.Authors
                .Select(a => new AuthorDto(
                    a.Id,
                    a.FirstName,
                    a.LastName,
                    a.Bio
                ))
                .ToListAsync();

            return Ok(authors);
        }

        // GET /api/authors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAuthor(int id)
        {
            var author = await _context.Authors
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.FirstName,
                    a.LastName,
                    a.Bio,
                    Books = a.Books.Select(b => new
                    {
                        b.Id,
                        b.Title,
                        b.Description,
                        b.PublishedDate
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (author == null)
            {
                return NotFound("Author not found");
            }

            return Ok(author);
        }

        // POST /api/authors
        [HttpPost]
        public async Task<ActionResult> CreateAuthor([FromBody] CreateAuthorDto dto)
        {
            var author = new Author
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Bio = dto.Bio,
                BirthDate = dto.BirthDate
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAuthor),
                new { id = author.Id },
                new AuthorDto(author.Id, author.FirstName, author.LastName, author.Bio)
            );
        }
        //PUT /api/authors/{id} — edytuj autora
        [HttpPut("{id}")]
        public async Task<ActionResult> EditAuthor(int id, [FromBody] UpdateAuthorDto dto)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound("Author not found");
            }

            author.FirstName = dto.FirstName;
            author.LastName = dto.LastName;
            author.Bio = dto.Bio;
            author.BirthDate = dto.BirthDate;  // tego brakowało

            await _context.SaveChangesAsync();
            return NoContent();
        }
       
        //DELETE /api/authors/{id} — usuń(tylko jeśli nie ma książek!)
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (author == null)
            {
                return NotFound("Author not found");
            }
            if (author.Books.Any())
            {
                return Conflict("Cannot delete author with existing books");
            }
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }   
}
