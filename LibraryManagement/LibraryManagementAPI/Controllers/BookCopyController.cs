﻿using EFCoreClasses;
using EFCoreClasses.Models;
using LibraryManagementAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
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
        [SwaggerResponse(201)]
        public async Task<ActionResult> CreateBookCopy([FromBody, Required] CreateBookCopyRequest request)
        {
            // request validation
            var book = await _context.Books.FirstOrDefaultAsync(b => b.SerialNumber == request.SerialNumber);

            if (book == null)
                return NotFound($"No book serial number {request.SerialNumber} was found in the database");

            // DTO validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // POST logic
            // if information on condition was not provided, it is considered as new by default
            if (string.IsNullOrEmpty(request.BookCondition))
                request.BookCondition = "As new";

            var bookCondition = await _context.BookConditions.FirstOrDefaultAsync(bc => bc.Condition == request.BookCondition);

            // if information on condition was not provided, it is considered empty
            if (request.Notes.IsNullOrEmpty())
                request.Notes = "";

            var bookCopy = new BookCopy
            {
                SerialNumber = request.SerialNumber,
                BookConditionID = bookCondition.ID,
                Notes = request.Notes
            };

            book.BookStock.TotalAmount++;
            book.BookStock.AvailableAmount++;

            _context.Add(bookCopy);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookCopies), new { id = bookCopy.ID }, null);
        }


        [HttpGet("filter")]
        [SwaggerResponse(200, Type = typeof(List<BookCopyResponse>))]
        public async Task<ActionResult<List<BookCopyResponse>>> GetBookCopies([FromQuery, Required] GetBookCopyRequest filter)
        {
            // GET by filter logic
            if (filter.Id == null && filter.SerialNumber == null && filter.Title == null && filter.Condition == null)
            {
                return BadRequest("At least one filter (Id, Serial Number, Title, Condition) must be provided.");
            }

            var bookcopies = await _context.BookCopies.Where(bc => (filter.Id == null || bc.ID == filter.Id)
                                                    && (filter.SerialNumber == null || bc.SerialNumber == filter.SerialNumber)
                                                    && (filter.Title == null || bc.Book.Title.Contains(filter.Title))
                                                    && (filter.Condition == null || bc.BookCondition.Condition == filter.Condition))
                                                .Select(bc => new BookCopyResponse
                                                {
                                                    Id = bc.ID,
                                                    SerialNumber = bc.SerialNumber,
                                                    Title = bc.Book.Title,
                                                    BookCondition = bc.BookCondition.Condition,
                                                    Notes = bc.Notes
                                                 })
                                                .ToListAsync();

            return Ok(bookcopies);
        }

        
        [HttpGet("list")]
        [SwaggerResponse(200, Type = typeof(PaginatedBookCopyResponse))] // tried to apply pagination due to the possiblity of a big number of data
        public async Task<ActionResult<PaginatedBookCopyResponse>> GetAllBookCopies([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // FromQuery Validation
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page number and page size must be greater than 0.");

            // GET logic
            var query = _context.BookCopies.Select(bc => new BookCopyResponse
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

            var response = new PaginatedBookCopyResponse
            {
                TotalBookCopies = totalBooks,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                BookCopies = bookCopies
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> UpdateBookCopy(int id, [FromBody, Required] UpdateBookCopyRequest request)
        {
            // FromRoute validation
            var bookCopy = await _context.BookCopies.FirstOrDefaultAsync(bc => bc.ID == id);

            if (bookCopy == null)
                return NotFound("Book copy not found");

            // DTO validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // PUT logic
            if (request.NewCondition == null && request.NewNotes == null)
                return BadRequest("At least one of 'Condition' or 'Notes' must be provided.");

            if (request.NewCondition != null)
            {
                var newCondition = await _context.BookConditions.FirstOrDefaultAsync(bc => bc.Condition == request.NewCondition);
                var oldConditionId = bookCopy.BookConditionID;

                if (newCondition.ID <= oldConditionId)
                {
                    ModelState.AddModelError("NewCondition", "Book condition must be worse than the current one");
                    return BadRequest(ModelState);
                }

                bookCopy.BookConditionID = newCondition.ID;
            }

            if (request.NewNotes != null)
                bookCopy.Notes = request.NewNotes;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> DeleteBookCopy(int id)
        {
            //FromRoute validation
            var bookCopy = await _context.BookCopies.FirstOrDefaultAsync(bc => bc.ID == id);

            if (bookCopy == null)
                return NotFound("Book copy not found.");

            // DELETE logic
            // check if book is rented
            var isCurrentlyRented = bookCopy.Rents.Any(r => r.RentReception == null);

            if (isCurrentlyRented)
                return Conflict("Cannot delete a rented book copy"); 

            // update bookstock before deleting book copy
            var bookStock = await _context.BookStocks.FirstOrDefaultAsync(bs => bs.SerialNumber == bookCopy.SerialNumber);

            bookStock.TotalAmount--;
            bookStock.AvailableAmount--;

            _context.BookCopies.Remove(bookCopy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
