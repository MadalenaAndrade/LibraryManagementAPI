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
    [Route("Rent")]
    public class RentController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public RentController(LibraryDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [SwaggerResponse(200, Type = typeof(RentResponse))]
        public async Task<IActionResult> CreateRent([FromBody, Required] RentRequest request)
        {
            if (request.ClientId == null && request.ClientNIF == null)
            {
                return BadRequest("A Client Id or NIF must be provided.");
            }

            if (request.BookSerialNumber == null && request.BookCopyId == null)
            {
                return BadRequest("A book serial number or book copy ID must be provided");
            }

            try
            {
                // check if book exists and is available
                var book = await _context.BookCopies.Include(bc => bc.Book)
                                                        .ThenInclude(bc => bc.BookStock)
                                                    .Include(bc => bc.Rents)
                                                        .ThenInclude(bc => bc.RentReception)
                                                    .Where(bc => (request.BookSerialNumber == null || bc.SerialNumber == request.BookSerialNumber)
                                                       && (request.BookCopyId == null || bc.ID == request.BookCopyId))
                                                    .FirstOrDefaultAsync();

                if (book == null)
                {
                    return NotFound("Book not found");
                }

                // check overall availability
                if (book.Book.BookStock.AvailableAmount < 1)
                {
                    return BadRequest("There's no book in stock at the moment");
                }


                // if bookcopy was introduced, check it's availability
                int bookCopyId;

                if (request.BookCopyId != null)
                {
                    var isRented = book.Rents.Any(r => r.RentReception == null);

                    if (isRented)
                    {
                        return BadRequest("Book copy is already rented");
                    }
                    bookCopyId = request.BookCopyId.Value;
                }
                // if user didn't specify bookCopy choose one not being rented
                else
                {
                    var availableBookCopy = await _context.BookCopies.Include(bc => bc.Rents)
                                                                        .ThenInclude(bc => bc.RentReception)
                                                                     .Where(bc => bc.SerialNumber == book.SerialNumber)
                                                                     .Where(bc => !bc.Rents.Any(r => r.RentReception == null)) //exclude rented copies
                                                                     .FirstOrDefaultAsync();
                    bookCopyId = availableBookCopy.ID;
                }

                // with the book being available check if client exists
                var client = await _context.Clients.Where(c => (request.ClientId == null || c.ID == request.ClientId)
                                                            && (request.ClientNIF == null || c.NIF == request.ClientNIF))
                                                   .FirstOrDefaultAsync();

                if (client == null)
                {
                    return NotFound("Client information not found. Please create a client file.");
                }

                // client can't rent new book if has another rented
                var hasActiveRent = await _context.Rents.Where(r => r.ClientID == client.ID && r.RentReception == null)
                                                        .AnyAsync();

                if (hasActiveRent)
                {
                    return BadRequest("Client already has a rented book that hasn't been returned");
                }

                // being data validated, add rent
                Rent rent;

                if (request.StartDate == null)
                {
                    rent = new Rent
                    {
                        ClientID = client.ID,
                        BookCopyID = bookCopyId,
                    };
                }
                else
                {
                    string[] validFormats = { "dd-MM-yyyy", "dd/MM/yyyy" };
                    DateTime startDate = DateTime.ParseExact(request.StartDate, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);

                    rent = new Rent
                    {
                        ClientID = client.ID,
                        BookCopyID = bookCopyId,
                        StartDate = startDate,
                        DueDate = startDate.AddDays(7)
                    };
                }

                await _context.Rents.AddAsync(rent);

                // update available stock
                book.Book.BookStock.AvailableAmount--;

                await _context.SaveChangesAsync();

                var response = new RentResponse
                {
                    ClientId = client.ID,
                    ClientName = client.Name,
                    BookCopyId = bookCopyId,
                    SerialNumber = book.SerialNumber,
                    BookTitle = book.Book.Title,
                    StartDate = rent.StartDate,
                    DueDate = rent.DueDate
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [SwaggerResponse(200, Type = typeof(RentResponse))]
        public async Task<IActionResult> GetAllRents(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page number and page size must be greater than 0.");

            try
            {
                var query = _context.Rents.Select(r => new RentResponse
                {
                    ClientId = r.ClientID,
                    ClientName = r.Client.Name,
                    BookCopyId = r.BookCopyID,
                    SerialNumber = r.BookCopy.SerialNumber,
                    BookTitle = r.BookCopy.Book.Title,
                    StartDate = r.StartDate,
                    DueDate = r.DueDate
                });

                // Calculate the quantity of itens 
                var totalRents = await query.AsNoTracking().CountAsync();

                var rents = await query.AsNoTracking()
                                       .Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();

                var response = new
                {
                    TotalRents = totalRents,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    Rents = rents
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
