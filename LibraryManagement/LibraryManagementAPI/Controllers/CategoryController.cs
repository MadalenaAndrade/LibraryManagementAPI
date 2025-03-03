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
    [Route("Category")]
    public class CategoryController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public CategoryController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerResponse(201)]
        public async Task<ActionResult> CreateCategories([FromBody, Required] CategoryRequest category)
        {
            // verifies first the validation of the DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Categories.Any(c => c.Name == category.Name))
            {
                ModelState.AddModelError("Name", $"Category with name '{category.Name}' already exists");
                return BadRequest(ModelState);
            }

            var newCategory = new Category
            {
                Name = category.Name
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryByID), new { id = newCategory.ID}, null);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(CategoryResponse))]
        public async Task<ActionResult<CategoryResponse>> GetCategoryByID(short id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound("Category not found");

            var response = new CategoryResponse
            {
                CategoryId = category.ID,
                Name = category.Name
            };

            return Ok(response);
        }

        [HttpGet("list")]
        [SwaggerResponse(200, Type = typeof(List<CategoryResponse>))]
        public async Task<ActionResult<List<CategoryResponse>>> GetAllCategories()
        {
                var response = await _context.Categories
                    .Select(c => new CategoryResponse
                    {
                        CategoryId = c.ID,
                        Name = c.Name
                    })
                    .ToListAsync();

                return Ok(response);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> UpdateCategory(short id, [FromBody, Required] CategoryRequest request)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound("Category not found");

            // initial DTO validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           
            if (_context.Categories.Any(c => c.Name == request.Name && c.ID != id))
            {
                ModelState.AddModelError("Name", $"Category with name '{request.Name}' already exists");
                return BadRequest(ModelState);
            }

            category.Name = request.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> DeleteCategory(short id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound("Category not found");

            // Check if there are categories associated to the author
            var isCategoryLinkedToBooks = await _context.BookCategories.AnyAsync(ba => ba.CategoryID == id);

            if (isCategoryLinkedToBooks)
                return BadRequest("Category cannot be deleted because there is at least one relationship to a book.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

