using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Azure;
using Azure.Core;
using EFCoreClasses;
using EFCoreClasses.Models;
using LibraryManagementAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("Books")]
    public class BookController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [SwaggerResponse(201)]
        public async Task<ActionResult> CreateBooks([FromBody, Required] CreateBookRequest request)
        {
            // initial dto validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //check if book already exists through sn
            if (_context.Books.Any(b => b.SerialNumber == request.SerialNumber))
            {
                ModelState.AddModelError("SerialNumber", $"Book with the serial number '{request.SerialNumber}' already exists");
                return BadRequest(ModelState);
            }

            // checks if publisher exists and adds if it is the case
            var publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Name == request.PublisherName);

            if (publisher == null)
            {
                publisher = new Publisher { Name = request.PublisherName };
                _context.Publishers.Add(publisher);
                await _context.SaveChangesAsync();
            }

            // create bookstock
            var bookStock = new BookStock
            {
                TotalAmount = request.TotalAmount,
                AvailableAmount = request.TotalAmount
            };

            // create book
            var newBook = new Book
            {
                SerialNumber = request.SerialNumber,
                Title = request.Title,
                Year = request.Year,
                FinePerDay = request.FinePerDay,
                Publisher = publisher,
                BookStock = bookStock
            };

            await _context.Books.AddRangeAsync(newBook);
            await _context.SaveChangesAsync();

            // adds bookCopies automatically
            var bookCopiesList = new List<BookCopy>();
            for (int i = 0; i < request.TotalAmount; i++)
            {
                var bookCopy = new BookCopy
                {
                    SerialNumber = request.SerialNumber,
                    BookConditionID = 1,
                };
                bookCopiesList.Add(bookCopy);
            }

            await _context.BookCopies.AddRangeAsync(bookCopiesList);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooks), new { serialNumber = newBook.SerialNumber }, null);
        }

        [HttpPost("{serialNumber}/authors")]
        [SwaggerResponse(201)]
        public async Task<ActionResult> AddAuthorsToBook(long serialNumber, [FromBody, Required] List<AuthorRequest> request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifies if serialNumber exists
            var book = await _context.Books.FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

            if (book == null)
                return NotFound($"Book with serial number '{serialNumber}' not found");


            // Verifies if authors and relationships exists or needs to be created.
            var newAuthors = new List<Author>();
            var newBookAuthors = new List<BookAuthor>();

            foreach (var authorRequest in request)
            {
                var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == authorRequest.Name);

                if (author == null)
                {
                    newAuthors.Add(new Author { Name = authorRequest.Name });
                }

                if (!_context.BookAuthors.Any(ba => ba.SerialNumber == serialNumber && ba.AuthorID == author.ID))
                {
                    newBookAuthors.Add(new BookAuthor
                    {
                        SerialNumber = serialNumber,
                        AuthorID = author.ID
                    });
                }
            }
            
            if (newAuthors.Any())
                _context.Authors.AddRange(newAuthors);

            if (newBookAuthors.Any())
                _context.BookAuthors.AddRange(newBookAuthors);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooks), new { serialNumber = book.SerialNumber}, null);
        }

        [HttpPost("{serialNumber}/categories")]
        [SwaggerResponse(201)]
        public async Task<ActionResult> AddCategoriesToBook(long serialNumber, [FromBody, Required] List<CategoryRequest> request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifies if serialNumber exists
            var book = await _context.Books.FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

            if (book == null)
                return NotFound($"Book with serial number '{serialNumber}' not found");


            // Verifies if categories and relationships exists or needs to be created.
            var newCategories = new List<Category>();
            var newBookCategories = new List<BookCategory>();

            foreach (var categoryRequest in request)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(a => a.Name == categoryRequest.Name);

                if (category == null)
                {
                    newCategories.Add(new Category { Name = categoryRequest.Name });
                }

                if (!_context.BookCategories.Any(ba => ba.SerialNumber == serialNumber && ba.CategoryID == category.ID))
                {
                    newBookCategories.Add(new BookCategory
                    {
                        SerialNumber = serialNumber,
                        CategoryID = category.ID
                    });
                }
            }

            if (newCategories.Any())
                _context.Categories.AddRange(newCategories);

            if (newBookCategories.Any())
                _context.BookCategories.AddRange(newBookCategories);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooks), new { serialNumber = book.SerialNumber }, null);
        }


        [HttpGet("filter")]
        [SwaggerResponse(200, Type = typeof(List<BookResponse>))]
        public async Task<ActionResult<List<BookResponse>>> GetBooks([FromQuery] GetBookRequest filter)
        {
            if (filter == null)
            {
                return BadRequest("At least one filter (Serial Number, Title, Year, Publisher, Author, or Category) must be provided.");
            }


            var query = _context.Books.Include(b => b.BookAuthors)
                                        .ThenInclude(b => b.Author)
                                      .Include(b => b.BookCategories)
                                        .ThenInclude(b => b.Category)
                                      .Include(b => b.Publisher)
                                      .Include(b => b.BookStock)
                                      .Where(b => (filter.SerialNumber == null || b.SerialNumber == filter.SerialNumber)
                                               && (filter.Title == null || b.Title.Contains(filter.Title))
                                               && (filter.Year == null || b.Year == filter.Year)
                                               && (filter.Publisher == null || b.Publisher.Name.Contains(filter.Publisher))
                                               && (filter.Author == null || b.BookAuthors.Any(ba => ba.Author.Name.Contains(filter.Author)))
                                               && (filter.Category == null || b.BookCategories.Any(bc => bc.Category.Name.Contains(filter.Category)))
                                             );
                                       
          
            var books = await query.ToListAsync();

            if (!books.Any())
                return NotFound("Book not found");

            var response = books.Select(b => new BookResponse
            {
                SerialNumber = b.SerialNumber,
                Title = b.Title,
                Year = b.Year,
                FinePerDay = b.FinePerDay,
                PublisherName = b.Publisher.Name,
                AuthorName = b.BookAuthors.Select(ba => ba.Author.Name).ToList(),
                CategoryName = b.BookCategories.Select(bc => bc.Category.Name).ToList(),
                TotalAmount = b.BookStock.TotalAmount,
                AvailableAmount = b.BookStock.AvailableAmount
            });

            return Ok(response);
        }
        

        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BookResponse))]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var response = await _context.Books
                    .Select(b => new BookResponse
                    {
                        SerialNumber = b.SerialNumber,
                        Title = b.Title,
                        Year = b.Year,
                        FinePerDay = b.FinePerDay,
                        PublisherName = b.Publisher.Name,
                        AuthorName = b.BookAuthors.Select(ba => ba.Author.Name).ToList(),
                        CategoryName = b.BookCategories.Select(bc => bc.Category.Name).ToList(),
                        TotalAmount = b.BookStock.TotalAmount,
                        AvailableAmount = b.BookStock.AvailableAmount
                    })
                    .ToListAsync();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("details")]
        [SwaggerResponse(200, Type = typeof(UpdateBookResponse))]
        public async Task<IActionResult> UpdateBook([FromQuery, Required] long serialNumber, [FromBody] UpdateBookRequest request)
        {
            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits");
            }

            if (request.Title == null && request.Year == null && request.FinePerDay == null && request.Publisher == null)
            {
                return BadRequest("Request body cannot be empty, provide a field to change (Title, Year, Fine per year, Publisher.");
            }

            try
            {
                //include Authors, Categories and BookStock for response
                var book = await _context.Books.Include(b => b.Publisher)
                                               .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

                if (book == null)
                {
                    return NotFound($"Book with Serial Number '{serialNumber}' not found.");
                }

                if (request.Title != null)
                {
                    if (book.Title == request.Title)
                    {
                        return BadRequest("The title provided is the same as the current one");
                    }
                    book.Title = request.Title;
                }

                if (request.Year.HasValue)
                {
                    if (book.Year == request.Year)
                    {
                        return BadRequest("The year provided is the same as the current one");
                    }
                    book.Year = request.Year.Value;
                }

                if (request.FinePerDay.HasValue)
                {
                    if (book.FinePerDay == request.FinePerDay)
                    {
                        return BadRequest("The fine per day provided is the same as the current one");
                    }
                    book.FinePerDay = request.FinePerDay.Value;
                }

                // check first if new publisher exists on Publisher, if not it creates
                if (request.Publisher != null)
                {
                    var existingPublisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Name == request.Publisher);

                    if (book.Publisher.Name == existingPublisher.Name)
                    {
                        return BadRequest("The publisher name provided is the same as the current one");
                    }

                    if (existingPublisher == null)
                    {
                        existingPublisher = new Publisher { Name = request.Publisher };
                        _context.Publishers.Add(existingPublisher);
                        await _context.SaveChangesAsync();
                    }

                    book.PublisherID = existingPublisher.ID;
                }

                _context.Books.Update(book);
                await _context.SaveChangesAsync();

                var response = new UpdateBookResponse
                {
                    SerialNumber = book.SerialNumber,
                    Title = book.Title,
                    Year = book.Year,
                    FinePerDay = book.FinePerDay,
                    PublisherName = book.Publisher.Name
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("author")]
        [SwaggerResponse(200, Type = typeof(UpdateBookAuthorResponse))]
        public async Task<IActionResult> UpdateBookAuthor([FromQuery, Required] long serialNumber, [FromBody, Required] UpdateBookAuthorRequest request)
        {
            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits.");
            }

            if (request.OldAuthorName.ToLower().Trim() == request.NewAuthorName.ToLower().Trim())
            {
                return BadRequest("The new author's name must be different from the current author's name.");
            }

            try
            {
                var book = await _context.Books.Include(b => b.BookAuthors)
                                               .ThenInclude(ba => ba.Author)
                                               .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

                if (book == null)
                {
                    return NotFound($"Book with Serial Number '{serialNumber}' not found.");
                }

                // checks if new author exists, add it to table if not
                var newAuthor = await _context.Authors.FirstOrDefaultAsync(p => p.Name.ToLower() == request.NewAuthorName.ToLower().Trim());
                
                if (newAuthor == null)
                {
                    newAuthor = new Author { Name = request.NewAuthorName };
                    _context.Authors.Add(newAuthor);
                    await _context.SaveChangesAsync();
                }

                // check if old author exists
                var bookAuthor = book.BookAuthors.FirstOrDefault(ba => ba.Author.Name.ToLower() == request.OldAuthorName.ToLower().Trim());

                if (bookAuthor != null)
                {
                    // If it exists, detach the entity to prevent multiple tracking issues
                    _context.Entry(bookAuthor).State = EntityState.Detached;

                    // remove old author relationship to avoid key conflits, as it will be later substituted by new author
                    _context.BookAuthors.Remove(bookAuthor);
                    await _context.SaveChangesAsync();
                }

                // Ensure EF is not tracking an existing BookAuthor instance
                var existingBookAuthor = await _context.BookAuthors.FirstOrDefaultAsync(ba => ba.SerialNumber == book.SerialNumber && ba.AuthorID == newAuthor.ID);

                if (existingBookAuthor == null)
                {
                    // add new BookAuthor relationship
                    var newBookAuthor = new BookAuthor
                    {
                        SerialNumber = book.SerialNumber,
                        AuthorID = newAuthor.ID
                    };
                    _context.BookAuthors.Add(newBookAuthor);

                    await _context.SaveChangesAsync();
                }

                var response = new UpdateBookAuthorResponse
                {
                    SerialNumber = book.SerialNumber,
                    Title = book.Title,
                    Year = book.Year,
                    AuthorNames = book.BookAuthors.Select(ba => ba.Author.Name).ToList()
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("category")]
        [SwaggerResponse(200, Type = typeof(UpdateBookCategoryResponse))]
        public async Task<IActionResult> UpdateBookCategory([FromQuery, Required] long serialNumber, [FromBody, Required] UpdateBookCategoryRequest request)
        {
            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits.");
            }

            if (request.OldCategoryName.ToLower().Trim() == request.NewCategoryName.ToLower().Trim())
            {
                return BadRequest("The new author's name must be different from the current author's name.");
            }

            try
            {
                var book = await _context.Books.Include(b => b.BookCategories)
                                               .ThenInclude(bc => bc.Category)
                                               .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

                if (book == null)
                {
                    return NotFound($"Book with Serial Number '{serialNumber}' not found.");
                }

                // checks if new category exists, add it to table if not
                var newCategory = await _context.Categories.FirstOrDefaultAsync(p => p.Name.ToLower() == request.NewCategoryName.ToLower().Trim());

                if (newCategory == null)
                {
                    newCategory = new Category { Name = request.NewCategoryName };
                    _context.Categories.Add(newCategory);
                    await _context.SaveChangesAsync();
                }

                // check if old category exists
                var bookCategory = book.BookCategories.FirstOrDefault(ba => ba.Category.Name.ToLower() == request.OldCategoryName.ToLower().Trim());

                if (bookCategory != null)
                {
                    // Detach the entity to prevent multiple tracking issues ----- not sure if I should do something like this, as I am quite "forcing" the relationship
                    _context.Entry(bookCategory).State = EntityState.Detached;

                    // remove old category relationship to avoid key conflits
                    _context.BookCategories.Remove(bookCategory);
                    await _context.SaveChangesAsync();
                }

                // Ensure EF is not tracking an existing BookCategory instance
                var existingBookCategory = await _context.BookCategories.FirstOrDefaultAsync(ba => ba.SerialNumber == book.SerialNumber && ba.CategoryID == newCategory.ID);

                if (existingBookCategory == null)
                {
                    // add new BookCategory relationship
                    var newBookCategory = new BookCategory
                    {
                        SerialNumber = book.SerialNumber,
                        CategoryID = newCategory.ID
                    };
                    _context.BookCategories.Add(newBookCategory);

                    await _context.SaveChangesAsync();
                }

                var response = new UpdateBookCategoryResponse
                {
                    SerialNumber = book.SerialNumber,
                    Title = book.Title,
                    Year = book.Year,
                    CategoryNames = book.BookCategories.Select(ba => ba.Category.Name).ToList()
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
        public async Task<IActionResult> DeleteBook([FromQuery, Required] long serialNumber)
        {
            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits");
            }

            try
            {
                // check if book exists
                var book = await _context.Books.Include(b => b.BookStock)
                                               .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

                if (book == null)
                {
                    return NotFound($"No book serial number {serialNumber} was found in the database");
                }

                // ensure book is not being rented
                if (book.BookStock.AvailableAmount < book.BookStock.TotalAmount)
                {
                    return BadRequest($"There are books {book.Title} being rented");
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return Ok($"Book '{book.Title}' deleted sucessfully");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("author")]
        [SwaggerResponse(200, Type = typeof(string))]
        public async Task<IActionResult> DeleteBookAuthor([FromQuery, Required] long serialNumber, [FromBody, Required] string authorName)
        {
            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits");
            }

            try
            {
                // check if book exists and related author exists
                var book = await _context.Books.Include(b => b.BookAuthors)
                                               .ThenInclude(b => b.Author)
                                               .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

                if (book == null)
                {
                    return NotFound($"No book serial number {serialNumber} was found in the database");
                }

                var bookAuthor = book.BookAuthors.FirstOrDefault(ba => ba.Author.Name == authorName);

                if (bookAuthor == null)
                {
                    return NotFound($"No author named '{authorName}' was found for this book.");
                }

                //check how many authors the book has, if only one it can't be deleted
                if (book.BookAuthors.Count() < 2)
                { 
                    return BadRequest("The book only has one author. Please update it instead of deleting.");
                }

                // delete bookAuthor entry
                _context.BookAuthors.Remove(bookAuthor);
                await _context.SaveChangesAsync();

                return Ok($"Author '{authorName}' was successfully removed from book '{book.Title}'.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("category")]
        [SwaggerResponse(200, Type = typeof(string))]
        public async Task<IActionResult> DeleteBookCategory([FromQuery, Required] long serialNumber, [FromBody, Required] string categoryName)
        {
            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits");
            }

            try
            {
                // check if book exists and related category exists
                var book = await _context.Books.Include(b => b.BookCategories)
                                               .ThenInclude(b => b.Category)
                                               .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

                if (book == null)
                {
                    return NotFound($"No book serial number {serialNumber} was found in the database");
                }

                var bookCategory = book.BookCategories.FirstOrDefault(ba => ba.Category.Name == categoryName);

                if (bookCategory == null)
                {
                    return NotFound($"No category named '{categoryName}' was found for this book.");
                }

                //check how many categories the book has, if only one it can't be deleted
                if (book.BookCategories.Count() < 2)
                {
                    return BadRequest("The book only has one category. Please update it instead of deleting.");
                }

                // delete bookCategory entry
                _context.BookCategories.Remove(bookCategory);
                await _context.SaveChangesAsync();

                return Ok($"Category '{categoryName}' was successfully removed from book '{book.Title}'.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
