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
    [Route("Client")]
    public class ClientController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public ClientController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerResponse(200, Type = typeof(ClientResponse))]
        public async Task<IActionResult> CreateClient([FromBody, Required] CreateClientRequest client)
        {
            
            try
            {
                var nifInDb = await _context.Clients.FirstOrDefaultAsync(c => c.NIF == client.NIF);

                if (nifInDb != null)
                {
                    return BadRequest($"A client with the NIF {client.NIF} already exists");
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

                var response = new ClientResponse
                {
                    Id = newClient.ID,
                    Name = newClient.Name,
                    DateOfBirth = newClient.DateOfBirth,
                    NIF = newClient.NIF,
                    Contact = newClient.Contact,
                    Address = newClient.Address
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [SwaggerResponse(200, Type = typeof(ClientResponse))]
        public async Task<IActionResult> CreateClient([FromQuery, Required] GetClientRequest filter)
        {
            if (filter.Id == null && filter.Name == null && filter.NIF == null)
            {
                return BadRequest("At least one filter (Id, Name, NIF) must be provided.");
            }

            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("list")]
        [SwaggerResponse(200, Type = typeof(ClientResponse))]
        public async Task<IActionResult> GetAllClients()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch]
        [SwaggerResponse(200, Type = typeof(ClientResponse))]
        public async Task<IActionResult> UpdateClient([FromQuery, Required] GetClientRequest filter, [FromBody, Required] UpdateClientRequest request)
        {
            if (filter.Id == null && filter.NIF == null)
            {
                return BadRequest("At Client Id or NIF must be provided.");
            }

            if (request.Contact == null && request.Address == null)
            {
                return BadRequest("At least one of 'Contact' or 'Address' must be provided.");
            }

            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => (filter.Id == null || c.ID == filter.Id)
                                                     && (filter.Name == null || c.Name.ToLower().Contains(filter.Name.ToLower().Trim()))
                                                     && (filter.NIF == null || c.NIF == filter.NIF));
                    

                if (client == null)
                {
                    return NotFound("No client was found matching the given filters.");
                }

                if (request.Contact.HasValue)
                {
                    client.Contact = request.Contact.Value;
                }

                if (request.Address != null)
                {
                    client.Address = request.Address;
                }

                await _context.SaveChangesAsync();

                var result = new ClientResponse
                {
                    Id = client.ID,
                    Name = client.Name,
                    DateOfBirth = client.DateOfBirth,
                    NIF = client.NIF,
                    Contact = client.Contact,
                    Address = client.Address
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ToDo Delete request, considering clients without debts
    }
}
