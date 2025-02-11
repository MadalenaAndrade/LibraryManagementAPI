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
    public class CategoryController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public CategoryController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerResponse(200, Type = typeof(CategoryResponse))]
        public async Task<IActionResult> CreateCategories([FromBody, Required] IEnumerable<CategoryRequest> categories)
        {
            if (categories == null || !categories.Any())
            {
                return BadRequest("At least one category must be provided.");
            }

            try
            {
                var categoriesInDb = await _context.Categories
                                            .Select(c => c.Name)
                                            .ToListAsync();

                var categoriesList = new List<Category>();

                foreach (var categoryRequest in categories)
                {
                    if (categoriesInDb.Contains(categoryRequest.Name))
                    {
                        throw new MyException($"Category with name '{categoryRequest.Name}' already exists");
                    }

                    categoriesList.Add(new Category { Name = categoryRequest.Name });
                }

                _context.Categories.AddRange(categoriesList);
                await _context.SaveChangesAsync();

                var response = categoriesList.Select(c => new CategoryResponse
                {
                    CategoryId = c.ID,
                    Name = c.Name
                });

                return CreatedAtAction(nameof(CreateCategories), response);
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
        [SwaggerResponse(200, Type = typeof(CategoryResponse))]
        public async Task<IActionResult> GetCategoryByID([FromQuery, Required] long id)
        {

            if (id <= 0)
            {
                return BadRequest("Category ID must be a positive integer");
            }

            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound("Category not found");
                }

                var response = new CategoryResponse
                {
                    CategoryId = category.ID,
                    Name = category.Name
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("categories-list")]
        [SwaggerResponse(200, Type = typeof(CategoryResponse))]
        public async Task<IActionResult> GetAllCategories()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch]
        [SwaggerResponse(200, Type = typeof(CategoryResponse))]
        public async Task<IActionResult> UpdateCategory([FromQuery, Required] long id, [FromBody, Required] CategoryRequest request)
        {
            if (id <= 0)
            {
                return BadRequest("Category ID must be a positive integer");
            }

            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                    return NotFound("Category not found");

                category.Name = request.Name;
                await _context.SaveChangesAsync();

                var response = new CategoryResponse
                {
                    CategoryId = category.ID,
                    Name = category.Name
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
        public async Task<IActionResult> DeleteCategory([FromQuery] long id)
        {
            if (id <= 0)
                return BadRequest("Category ID must be a positive integer");

            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                    return NotFound("Category not found");

                CategoryResponse categoryDeleted = new CategoryResponse
                {
                    CategoryId = category.ID,
                    Name = category.Name
                };

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok($"Category '{categoryDeleted.Name}' was deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
