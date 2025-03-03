using System.ComponentModel.DataAnnotations;
using Azure.Core;
using EFCoreClasses;
using EFCoreClasses.Models;
using LibraryManagementAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("Author")]
    public class AuthorController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public AuthorController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerResponse(201)]
        public async Task<ActionResult> CreateAuthors([FromBody, Required] AuthorRequest author)
        {
            // initial dto validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Authors.Any(a => a.Name == author.Name))
            {
                ModelState.AddModelError("Name", $"Author with name '{author.Name}' already exists");
                return BadRequest(ModelState);
            }

            var newAuthor = new Author
            {
                Name = author.Name
            };

            _context.Authors.Add(newAuthor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthorByID), new { id = newAuthor.ID }, null);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(AuthorResponse))]
        public async Task<ActionResult<AuthorResponse>> GetAuthorByID(long id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
                return NotFound("Author not found");

            var response = new AuthorResponse
            {
                AuthorId = author.ID,
                Name = author.Name
            };

            return Ok(response);
        }

        [HttpGet("list")]
        [SwaggerResponse(200, Type = typeof(List<AuthorResponse>))]
        public async Task<ActionResult<List<AuthorResponse>>> GetAllAuthors()
        {
            var response = await _context.Authors
                .Select(a => new AuthorResponse
                {
                    AuthorId = a.ID,
                    Name = a.Name
                })
                .ToListAsync();

            return Ok(response);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> UpdateAuthor(long id, [FromBody, Required] AuthorRequest request)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
                return NotFound("Author not found");

            // initial dto validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (_context.Authors.Any(a => a.Name == request.Name && a.ID != id))
            {
                ModelState.AddModelError("Name", $"Author with name '{request.Name}' already exists");
                return BadRequest(ModelState);
            }

            author.Name = request.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> DeleteAuthor(long id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
                return NotFound("Author not found");

            // Check if there are books associated to the author
            var isAuthorLinkedToBooks = await _context.BookAuthors.AnyAsync(ba => ba.AuthorID == id);

            if (isAuthorLinkedToBooks)
                return BadRequest("Author cannot be deleted because there is at least one relationship to a book.");

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
