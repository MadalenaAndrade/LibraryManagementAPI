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
    [Route("Rents")]
    public class RentController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public RentController(LibraryDbContext context)
        {
            _context = context;
        }


        [HttpPost("clientId")]
        [SwaggerResponse(201)]
        public async Task<IActionResult> CreateRent(int clientId, [FromBody, Required] RentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (clientId <= 0)
                return BadRequest("Client ID must be a positive integer");

            if (request.BookSerialNumber == null && request.BookCopyId == null)
                return BadRequest("A book serial number or book copy ID must be provided");

                // check if book exists and is available
                var book = await _context.BookCopies.Include(bc => bc.Book)
                                                        .ThenInclude(bc => bc.BookStock)
                                                    .Include(bc => bc.Rents)
                                                        .ThenInclude(bc => bc.RentReception)
                                                    .Where(bc => (request.BookSerialNumber == null || bc.SerialNumber == request.BookSerialNumber)
                                                       && (request.BookCopyId == null || bc.ID == request.BookCopyId))
                                                    .FirstOrDefaultAsync();

                if (book == null)
                    return NotFound("Book not found");


                // check overall availability
                if (book.Book.BookStock.AvailableAmount <= 0)
                    return BadRequest("There's no book in stock at the moment");


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
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.ID == clientId);

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


                return CreatedAtAction(nameof(GetRentByID), new { id = rent.ID }, null);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(RentResponse))]
        public async Task<ActionResult<AuthorResponse>> GetRentByID(long id)
        {

            if (id <= 0)
                return BadRequest("Rent ID must be a positive integer");

            var rent = await _context.Rents.Include(r => r.Client)
                                           .Include(r => r.BookCopy)
                                            .ThenInclude(bc => bc.Book)
                                           .FirstOrDefaultAsync(r => r.ID == id);

            if (rent == null)
                return NotFound("Rent not found");

            var response = new RentResponse
            {
                ClientId = rent.ClientID,
                ClientName = rent.Client.Name,
                BookCopyId = rent.BookCopyID,
                SerialNumber = rent.BookCopy.SerialNumber,
                BookTitle = rent.BookCopy.Book.Title,
                StartDate = rent.StartDate,
                DueDate = rent.DueDate
            };

            return Ok(response);
        }


        [HttpGet]
        [SwaggerResponse(200, Type = typeof(List<RentResponse>))]
        public async Task<ActionResult<List<RentResponse>>> GetAllRents(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page number and page size must be greater than 0.");


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

    }
}
