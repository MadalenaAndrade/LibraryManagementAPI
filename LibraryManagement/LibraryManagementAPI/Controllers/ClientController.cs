using EFCoreClasses;
using EFCoreClasses.Models;
using LibraryManagementAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("Clients")]
    public class ClientController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public ClientController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerResponse(201)]
        public async Task<ActionResult> CreateClient([FromBody, Required] CreateClientRequest client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nifInDb = await _context.Clients.FirstOrDefaultAsync(c => c.NIF == client.NIF);

            if (nifInDb != null)
            {
                ModelState.AddModelError("NIF", $"A client with the NIF {client.NIF} already exists");
                return BadRequest(ModelState);
            }

            string[] validFormats = { "dd-MM-yyyy", "dd/MM/yyyy" };
            DateTime parsedDate = DateTime.ParseExact(client.DateOfBirth, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateOnly clientDate = DateOnly.FromDateTime(parsedDate);

            var newClient = new Client
            {
                Name = client.Name,
                DateOfBirth = clientDate,
                NIF = client.NIF,
                Contact = client.Contact,
                Address = client.Address
            };

            _context.Clients.Add(newClient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClients), new { id = newClient.ID }, null);
        }

        [HttpGet("filter")]
        [SwaggerResponse(200, Type = typeof(List<ClientResponse>))]
        public async Task<ActionResult<List<ClientResponse>>> GetClients([FromQuery, Required] GetClientRequest filter)
        {
            if (filter.Id == null && filter.Name == null && filter.NIF == null)
            {
                return BadRequest("At least one filter (Id, Name, NIF) must be provided.");
            }


            var query = _context.Clients.Where(c => (filter.Id == null || c.ID == filter.Id)
                                                 && (filter.Name == null || c.Name.ToLower().Contains(filter.Name.ToLower().Trim()))
                                                 && (filter.NIF == null || c.NIF == filter.NIF))
                                        .Select(c => new ClientResponse
                                        {
                                            Id = c.ID,
                                            Name = c.Name,
                                            DateOfBirth = c.DateOfBirth,
                                            NIF = c.NIF,
                                            Contact = c.Contact,
                                            Address = c.Address
                                        });

            var result = await query.ToListAsync();

            if (result == null || !result.Any())
            {
                return NotFound("No clients found matching the given filters.");
            }

            return Ok(result);
        }

        [HttpGet]
        [SwaggerResponse(200, Type = typeof(List<ClientResponse>))]
        public async Task<ActionResult<List<ClientResponse>>> GetAllClients()
        {

            var response = await _context.Clients
                .Select(c => new ClientResponse
                {
                    Id = c.ID,
                    Name = c.Name,
                    DateOfBirth = c.DateOfBirth,
                    NIF = c.NIF,
                    Contact = c.Contact,
                    Address = c.Address
                })
                .ToListAsync();

            return Ok(response);
        }


        [HttpPut("{id}")]
        [SwaggerResponse(204)]
        public async Task<IActionResult> UpdateClient(int id, [FromBody, Required] UpdateClientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id < 1)
                return BadRequest("Id must be positive");

            if (request.Contact == null && request.Address == null)
            {
                return BadRequest("At least one of 'Contact' or 'Address' must be provided.");
            }

            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound("Client not found.");

            if (request.Contact.HasValue)
            {
                client.Contact = request.Contact.Value;
            }

            if (request.Address != null)
            {
                client.Address = request.Address;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> DeleteClient(int id)
        {
            if (id < 1)
                return BadRequest("Id must be positive");

            var client = await _context.Clients.Include(c => c.Rents)
                                               .FirstOrDefaultAsync(c => c.ID == id);

            if (client == null)
                return NotFound("Client not found.");

            // only delete if client has never rented
            if (client.Rents.Any())
                return BadRequest("Cannot delete a client that has rented a book previously");

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
