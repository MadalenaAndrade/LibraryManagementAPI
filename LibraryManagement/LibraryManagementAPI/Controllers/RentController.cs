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
        [SwaggerResponse(201)]
        public async Task<ActionResult> CreateRent([FromBody, Required] RentRequest request)
        {
            //Client validation
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ID == request.ClientId);

            if (client == null)
                return NotFound("Client information not found. Please create a client file.");

            // DTO validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            // POST logic, check if book exists
            if (request.BookSerialNumber == null && request.BookCopyId == null)
            {
                ModelState.AddModelError("BookSerialNumber", "A book serial number or book copy ID must be provided");
                ModelState.AddModelError("BookCopyId", "A book serial number or book copy ID must be provided");
                return BadRequest(ModelState); //adds to dto validation
            }

            // check if book exists and is available
            var book = await _context.BookCopies.Where(bc => (request.BookSerialNumber == null || bc.SerialNumber == request.BookSerialNumber)
                                                   && (request.BookCopyId == null || bc.ID == request.BookCopyId))
                                                .FirstOrDefaultAsync();

            if (book == null)
                return NotFound("Book not found");


            // check overall availability
            if (book.Book.BookStock.AvailableAmount <= 0)
            {
                ModelState.AddModelError("AvailableAmount", "There's no book in stock at the moment");
                return BadRequest(ModelState);
            }
        
            // if bookcopy was introduced, check it's availability
            int bookCopyId;

            if (request.BookCopyId != null)
            {
                var isRented = book.Rents.Any(r => r.RentReception == null);

                if (isRented)
                {
                    ModelState.AddModelError("BookCopyId", "Book copy is already rented");
                    return BadRequest(ModelState);
                }
                bookCopyId = request.BookCopyId.Value;
            }
            // if user didn't specify bookCopy choose one not being rented
            else
            {
                var availableBookCopy = await _context.BookCopies.Where(bc => bc.SerialNumber == book.SerialNumber)
                                                                 .Where(bc => !bc.Rents.Any(r => r.RentReception == null)) //exclude rented copies
                                                                 .FirstOrDefaultAsync();
                bookCopyId = availableBookCopy.ID;
            }

            // client can't rent new book if has another rented
            var hasActiveRent = await _context.Rents.Where(r => r.ClientID == client.ID && r.RentReception == null)
                                                    .AnyAsync();

            if (hasActiveRent)
            {
                ModelState.AddModelError("ClientID", "Client already has a rented book that hasn't been returned");
                return BadRequest(ModelState);
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
           _context.Rents.Add(rent);

            // update available stock
            book.Book.BookStock.AvailableAmount--;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRentByID), new { id = rent.ID }, null);
        }

        [HttpPost("Reception")]
        [SwaggerResponse(201)]
        public async Task<ActionResult> CreateRentReception([FromBody, Required] RentReceptionRequest request)
        {
            // Rent validation
            var rent = await _context.Rents.FirstOrDefaultAsync(r => r.ID == request.RentId);

            if (rent == null)
                return NotFound("No rented book was found");

            // DTO validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            // POST logic
            // check if book was not already delivered
            if (rent.RentReception != null)
            {
                ModelState.AddModelError("RentReception", "This rent has already been closed");
                return BadRequest(ModelState);
            }
                

            // Check if return Date has been inputed, if it has been, it is converts to DateTime, if not the default value of "now" is given
            // and validates date introduced
            DateTime returnedDate;
            string[] validFormats = { "dd-MM-yyyy", "dd/MM/yyyy" };

            if (!string.IsNullOrEmpty(request.ReturnDate))
                returnedDate = DateTime.ParseExact(request.ReturnDate, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            else
                returnedDate = DateTime.Now;

            if (returnedDate < rent.StartDate)
            {
                ModelState.AddModelError("ReturnDate", "Return date cannot be before the rental start date.");
                return BadRequest(ModelState);
            }
                
            // Calculate TotalFine depending on the condition received
            var receivedBookCondition = await _context.BookConditions.FirstOrDefaultAsync(bc => bc.Condition.ToLower() == request.ReceivedCondition.ToLower().Trim());
            var originalBookCondition = await _context.BookConditions.FirstOrDefaultAsync(bc => bc.ID == rent.BookCopy.BookConditionID);

            if (receivedBookCondition.ID < originalBookCondition.ID)
            {
                ModelState.AddModelError("ReceivedCondition", "The book cannot be returned in a better condition than it was rented.");
                return BadRequest(ModelState);
            }

            decimal totalFine = 0;

            // fine for returning late
            if (returnedDate > rent.DueDate)
            {
                var daysLate = (returnedDate - rent.DueDate).Days;
                totalFine += daysLate * rent.BookCopy.Book.FinePerDay * receivedBookCondition.FineModifier;
            }

            // fine if condition worsening
            if (receivedBookCondition.ID > originalBookCondition.ID)
            {
                totalFine += rent.BookCopy.Book.FinePerDay * (receivedBookCondition.FineModifier - originalBookCondition.FineModifier);
            }

            // add new rentReception
            var reception = new RentReception
            {
                RentID = rent.ID,
                ReturnDate = returnedDate,
                ReceivedConditionID = receivedBookCondition.ID,
                TotalFine = totalFine
            };

            _context.RentReceptions.Add(reception);

            //update book stock
            rent.BookCopy.Book.BookStock.AvailableAmount++;

            // update book condition if it was different
            if (originalBookCondition.ID != receivedBookCondition.ID)
                rent.BookCopy.BookCondition = receivedBookCondition;  

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRentReceptionByID), new { id = reception.RentID }, null);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(RentResponse))]
        public async Task<ActionResult<RentResponse>> GetRentByID(long id)
        {
            // FromRoute validation
            var rent = await _context.Rents.FirstOrDefaultAsync(r => r.ID == id);

            if (rent == null)
                return NotFound("Rent not found");

            // GET logic
            var response = new RentResponse
            {
                RentId = rent.ID,
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

        [HttpGet("{id}/Reception")]
        [SwaggerResponse(200, Type = typeof(RentReceptionResponse))]
        public async Task<ActionResult<RentReceptionResponse>> GetRentReceptionByID(long id)
        {
            // FromRoute validation
            var rentReception = await _context.RentReceptions.FirstOrDefaultAsync(rr => rr.RentID == id);

            if (rentReception == null)
                return NotFound("Rent reception not found");

            // GET logic
            var response = new RentReceptionResponse
            {
                RentId = rentReception.RentID,
                ReturnDate = rentReception.ReturnDate,
                ReceivedCondition = rentReception.BookCondition.Condition,
                TotalFine = rentReception.TotalFine
            };

            return Ok(response);
        }

        [HttpGet("list")]
        [SwaggerResponse(200, Type = typeof(PaginatedRentResponse))]
        public async Task<ActionResult<PaginatedRentResponse>> GetAllRents(int pageNumber = 1, int pageSize = 10)
        {
            // FromQuery pagination validation
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page number and page size must be greater than 0.");

            // GET logic
            var query = _context.Rents.Select(r => new RentResponse
            {
                RentId = r.ID,
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

        [HttpGet("ReceptionsList")]
        [SwaggerResponse(200, Type = typeof(PaginatedRentReceptionResponse))]
        public async Task<ActionResult<PaginatedRentReceptionResponse>> GetAllRentReceptions(int pageNumber = 1, int pageSize = 10)
        {
            // FromQuery pagination validation
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page number and page size must be greater than 0.");

            // GET logic
            var query = _context.RentReceptions.Select(rr => new RentReceptionResponse
            {
                RentId = rr.RentID,
                ReturnDate = rr.ReturnDate,
                ReceivedCondition = rr.BookCondition.Condition,
                TotalFine = rr.TotalFine
            });

            // Calculate the quantity of itens 
            var totalRentReceptions = await query.AsNoTracking().CountAsync();

            var rentReceptions = await query.AsNoTracking()
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var response = new
            {
                TotalRentReceptions = totalRentReceptions,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                RentReceptions = rentReceptions
            };

            return Ok(response);
        }
    }
}
