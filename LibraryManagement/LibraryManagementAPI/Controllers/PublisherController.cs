using System.ComponentModel.DataAnnotations;
using EFCoreClasses;
using EFCoreClasses.Models;
using LibraryManagementAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublisherController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public PublisherController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerResponse(200, Type = typeof(PublisherResponse))]
        public async Task<IActionResult> CreatePublishers([FromBody, Required] IEnumerable<PublisherRequest> publishers)
        {
            if (publishers == null || !publishers.Any())
            {
                return BadRequest("At least one publisher must be provided.");
            }

            try
            {
                var publishersInDb = await _context.Publishers
                                            .Select(p => p.Name)
                                            .ToListAsync();

                var publishersList = new List<Publisher>();

                foreach (var publisherRequest in publishers)
                {
                    if (publishersInDb.Contains(publisherRequest.Name))
                    {
                        throw new MyException($"Publishers with name '{publisherRequest.Name}' already exists");
                    }

                    publishersList.Add(new Publisher { Name = publisherRequest.Name });
                }

                _context.Publishers.AddRange(publishersList);
                await _context.SaveChangesAsync();

                var response = publishersList.Select(p => new PublisherResponse
                {
                    PublisherId = p.ID,
                    Name = p.Name
                });

                return CreatedAtAction(nameof(CreatePublishers), response);
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
        [SwaggerResponse(200, Type = typeof(PublisherResponse))]
        public async Task<IActionResult> GetPublisherByID([FromQuery, Required] int id)
        {

            if (id <= 0)
            {
                return BadRequest("Publisher ID must be a positive integer");
            }

            try
            {
                var publisher = await _context.Publishers.FindAsync(id);

                if (publisher == null)
                {
                    return NotFound("Publisher not found");
                }

                var response = new PublisherResponse
                {
                    PublisherId = publisher.ID,
                    Name = publisher.Name
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("list")]
        [SwaggerResponse(200, Type = typeof(PublisherResponse))]
        public async Task<IActionResult> GetAllPublishers()
        {
            try
            {
                var response = await _context.Publishers
                    .Select(p => new PublisherResponse
                    {
                        PublisherId = p.ID,
                        Name = p.Name
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
        [SwaggerResponse(200, Type = typeof(PublisherResponse))]
        public async Task<IActionResult> UpdatePublisher([FromQuery, Required] long id, [FromBody, Required] PublisherRequest request)
        {
            if (id <= 0)
            {
                return BadRequest("Publisher ID must be a positive integer");
            }

            try
            {
                var publisher = await _context.Publishers.FindAsync(id);

                if (publisher == null)
                    return NotFound("Publisher not found");

                publisher.Name = request.Name;
                await _context.SaveChangesAsync();

                var response = new PublisherResponse
                {
                    PublisherId = publisher.ID,
                    Name = publisher.Name
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
        public async Task<IActionResult> DeletePublisher([FromQuery] long id)
        {
            if (id <= 0)
                return BadRequest("Publisher ID must be a positive integer");

            try
            {
                var publisher = await _context.Publishers.FindAsync(id);

                if (publisher == null)
                    return NotFound("Publisher not found");

                PublisherResponse publisherDeleted = new PublisherResponse
                {
                    PublisherId = publisher.ID,
                    Name = publisher.Name
                };

                _context.Publishers.Remove(publisher);
                await _context.SaveChangesAsync();

                return Ok($"Publisher '{publisherDeleted.Name}' was deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}

