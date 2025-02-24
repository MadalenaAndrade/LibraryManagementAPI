﻿using System.ComponentModel.DataAnnotations;
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
    [Route("Publishers")]
    public class PublisherController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public PublisherController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerResponse(201)]
        public async Task<ActionResult> CreatePublishers([FromBody, Required] PublisherRequest publisher)
        {
            // verifies first the validation of the DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Publishers.Any(p => p.Name == publisher.Name))
            {
                ModelState.AddModelError("Name", $"Publisher with name '{publisher.Name}' already exists");
                return BadRequest(ModelState);
            }

            var newPublisher = new Publisher
            {
                Name = publisher.Name
            };

            _context.Publishers.Add(newPublisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPublisherByID), new { id = newPublisher.ID }, null);
        }


        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(PublisherResponse))]
        public async Task<ActionResult<PublisherResponse>> GetPublisherByID(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Publisher ID must be a positive integer");
            }

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


        [HttpGet]
        [SwaggerResponse(200, Type = typeof(List<PublisherResponse>))]
        public async Task<ActionResult<List<PublisherResponse>>> GetAllPublishers()
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

        [HttpPut("{id}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> UpdatePublisher(int id, [FromBody, Required] PublisherRequest request)
        {
            if (id <= 0)
            {
                return BadRequest("Publisher ID must be a positive integer");
            }

            var publisher = await _context.Publishers.FindAsync(id);

            if (publisher == null)
                return NotFound("Publisher not found");

            if(_context.Publishers.Any(p => p.Name == request.Name && p.ID != id))
            {
                ModelState.AddModelError("Name", $"Publisher's name '{request.Name}' already exists");
                return BadRequest(ModelState);
            }

            publisher.Name = request.Name;
            await _context.SaveChangesAsync();

            var response = new PublisherResponse
            {
                PublisherId = publisher.ID,
                Name = publisher.Name
            };

            return NoContent();
        }


        [HttpDelete("{id}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> DeletePublisher(int id)
        {
            if (id <= 0)
                return BadRequest("Publisher ID must be a positive integer");

            var publisher = await _context.Publishers.FindAsync(id);

            if (publisher == null)
                return NotFound("Publisher not found");

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

