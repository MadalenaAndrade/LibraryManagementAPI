using EFCoreClasses;
using EFCoreClasses.Models;
using LibraryManagementAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("BookCopy")]
    public class BookCopyController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BookCopyController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerResponse(200, Type = typeof(BookCopyResponse))]
        public async Task<IActionResult> CreateBookCopy([FromBody, Required] BookCopyRequest request)
        {
            try
            {
                // check if book exists
                var book = await _context.Books.Include(b => b.BookStock)
                                               .FirstOrDefaultAsync(b => b.SerialNumber == request.SerialNumber);

                if (book == null)
                {
                    return NotFound($"No book serial number {request.SerialNumber} was found in the database");
                }

                // if information on condition or notes were provided, it is considered as new by default
                if (string.IsNullOrEmpty(request.BookCondition))
                {
                    request.BookCondition = "As new";
                }

                var bookCondition = await _context.BookConditions.FirstAsync(bc => bc.Condition == request.BookCondition);

                if (request.Notes.IsNullOrEmpty())
                {
                    request.Notes = "";
                }

                var bookCopyList = new List<BookCopy>();

                for (int i = 0; i < request.Amount; i++)
                {
                    bookCopyList.Add(new BookCopy
                    {
                        SerialNumber = request.SerialNumber,
                        BookConditionID = bookCondition.ID,
                        Notes = request.Notes
                    });
                    book.BookStock.TotalAmount++;
                    book.BookStock.AvailableAmount++;
                }

                _context.AddRange(bookCopyList);
                await _context.SaveChangesAsync();

                var response = new BookCopyResponse
                {
                    SerialNumber = request.SerialNumber,
                    Amount = request.Amount,
                    BookCondition = bookCondition.Condition,
                    Notes = request.Notes
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        [SwaggerResponse(200, Type = typeof(GetBookCopyResponse))]
        public async Task<IActionResult> GetBookCopyById([FromQuery, Required] GetBookCopyRequest filter)
        {
            if (filter.Id == null && filter.SerialNumber == null && filter.Title == null && filter.Condition == null)
            {
                return BadRequest("At least one filter (Id, Serial Number, Title, Condition) must be provided.");
            }

            try
            {
                var query = _context.BookCopies.Include(bc => bc.Book)
                                               .Include(bc => bc.BookCondition)
                                               .Where(bc => (filter.Id == null || bc.ID == filter.Id)
                                                        && (filter.SerialNumber == null || bc.SerialNumber == filter.SerialNumber)
                                                        && (filter.Title == null || bc.Book.Title == filter.Title)
                                                        && (filter.Condition == null || bc.BookCondition.Condition == filter.Condition))
                                               .Select(bc => new GetBookCopyResponse
                                               {
                                                   Id = bc.ID,
                                                   SerialNumber = bc.SerialNumber,
                                                   Title = bc.Book.Title,
                                                   BookCondition = bc.BookCondition.Condition,
                                                   Notes = bc.Notes
                                               });

                var result = await query.ToListAsync();

                if (result == null || !result.Any())
                {
                    return NotFound("No book copies found matching the given filters.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // tried to apply pagination due to the possiblity of a big number of data
        [HttpGet("list")]
        [SwaggerResponse(200, Type = typeof(GetBookCopyResponse))]
        public async Task<IActionResult> GetAllBookCopies(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page number and page size must be greater than 0.");

            try
            {
                var query = _context.BookCopies.Select(bc => new GetBookCopyResponse
                {
                    Id = bc.ID,
                    SerialNumber = bc.SerialNumber,
                    Title = bc.Book.Title,
                    BookCondition = bc.BookCondition.Condition,
                    Notes = bc.Notes
                });

                // Calculate the quantity of itens 
                var totalBooks = await query.AsNoTracking().CountAsync();
                
                var bookCopies = await query.AsNoTracking()
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();

                var response = new
                {
                    TotalBookCopies = totalBooks,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    BookCopies = bookCopies
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateBookCondition([FromQuery, Required] int id, [FromBody, Required] UpdateBookCopyRequest request)
        {
            if (id < 1)
            {
                return BadRequest("Id must be positive");
            }

            if (request.NewCondition == null && request.NewNotes == null)
            {
                return BadRequest("At least one of 'Condition' or 'Notes' must be provided.");
            }

            try
            {
                var bookCopy = await _context.BookCopies.Include(bc => bc.Book)
                                                        .Include(bc => bc.BookCondition)
                                                        .FirstOrDefaultAsync(bc => bc.ID == id);

                if (bookCopy == null)
                {
                    return NotFound("Book copy not found");
                }


                if (request.NewCondition != null)
                {
                    var newCondition = await _context.BookConditions.FirstOrDefaultAsync(bc => bc.Condition == request.NewCondition);
                    var oldConditionId = bookCopy.BookConditionID;

                    if (newCondition.ID <= oldConditionId)
                    {
                        return BadRequest("Book condition must be worse than the current one");
                    }

                    bookCopy.BookConditionID = newCondition.ID;
                }

                if (request.NewNotes != null)
                {
                    bookCopy.Notes = request.NewNotes;
                }

                await _context.SaveChangesAsync();

                var result = new GetBookCopyResponse
                {
                    Id = bookCopy.ID,
                    SerialNumber = bookCopy.SerialNumber,
                    Title = bookCopy.Book.Title,
                    BookCondition = bookCopy.BookCondition.Condition,
                    Notes = bookCopy.Notes
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        // ToDo: delete BookCopy if not rented
    }
}
