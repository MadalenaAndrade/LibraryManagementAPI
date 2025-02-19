using System.ComponentModel.DataAnnotations;
using Azure.Core;
using EFCoreClasses;
using EFCoreClasses.Models;
using LibraryManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public AuthorController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerResponse(200, Type = typeof(AuthorResponse))]
        public async Task<IActionResult> CreateAuthors([FromBody, Required] IEnumerable<AuthorRequest> authors)
        {
            if (authors == null || !authors.Any())
            {
                return BadRequest("At least one author must be provided.");
            }

            try
            {
                var authorsInDb = await _context.Authors
                                            .Select(a => a.Name)
                                            .ToListAsync();
                
                var authorsList = new List<Author>();

                foreach (var authorRequest in authors)
                {
                    if (authorsInDb.Contains(authorRequest.Name))
                    {
                        throw new MyException($"Author with name '{authorRequest.Name}' already exists");
                    }

                    authorsList.Add(new Author { Name = authorRequest.Name });
                }

                _context.Authors.AddRange(authorsList);
                await _context.SaveChangesAsync();

                var response = authorsList.Select(a => new AuthorResponse
                {
                    AuthorId = a.ID,
                    Name = a.Name
                });

                return CreatedAtAction(nameof(CreateAuthors), response);
            }
            catch (MyException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [SwaggerResponse(200, Type = typeof(AuthorResponse))]
        public async Task<IActionResult> GetAuthorByID([FromQuery, Required] long id)
        {

            if (id <= 0)
            {
                return BadRequest("Author ID must be a positive integer");
            }

            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    return NotFound("Author not found");
                }

                var response = new AuthorResponse
                {
                    AuthorId = author.ID,
                    Name = author.Name
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("list")]
        [SwaggerResponse(200, Type = typeof(AuthorResponse))]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch] // Patch is a better option? I only want to update the author name and only if ID exists
        [SwaggerResponse(200, Type = typeof(AuthorResponse))]
        public async Task<IActionResult> UpdateAuthor([FromQuery, Required] long id, [FromBody, Required] AuthorRequest request)
        {
            if (id <= 0)
            {
                return BadRequest("Author ID must be a positive integer");
            }

            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                    return NotFound("Author not found");

                author.Name = request.Name;
                _context.Authors.Update(author);

                await _context.SaveChangesAsync();

                var response = new AuthorResponse
                {
                    AuthorId = author.ID,
                    Name = author.Name
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [SwaggerResponse(200, Type = typeof(string))]
        public async Task<IActionResult> DeleteAuthor([FromQuery, Required] long id)
        {
            if (id <= 0)
                return BadRequest("Author ID must be a positive integer");

            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                    return NotFound("Author not found");

                AuthorResponse authorDeleted = new AuthorResponse
                {
                    AuthorId = author.ID,
                    Name = author.Name
                };

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return Ok($"Author '{authorDeleted.Name}' was deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
