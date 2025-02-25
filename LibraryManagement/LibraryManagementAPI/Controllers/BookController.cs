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

        [HttpPost("{serialNumber}/author")]
        [SwaggerResponse(201)]
        public async Task<ActionResult> AddAuthorsToBook(long serialNumber, [FromBody, Required] AuthorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifies if serialNumber exists
            var book = await _context.Books.FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

            if (book == null)
                return NotFound($"Book with serial number '{serialNumber}' not found");

            // get author in db
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == request.Name);

            // if author doesn't exist it creates a new one
            if (author == null)
            {
                author = new Author { Name = request.Name };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync(); //saves to create author id 
            }

            // check if relationship between book and author already exists, if it exist returns bad request
            var existingRelation = await _context.BookAuthors.AnyAsync(ba => ba.SerialNumber == serialNumber && ba.AuthorID == author.ID);

            if (existingRelation)
            {
                ModelState.AddModelError("Authors", $"Author '{request.Name}' is already associated with this book.");
                return BadRequest(ModelState);
            }

            // if relationship doesn't exist it adds a new BookAuthor
            var bookAuthor = new BookAuthor
            {
                SerialNumber = serialNumber,
                AuthorID = author.ID
            };

            _context.BookAuthors.AddRange(bookAuthor);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetBooks), new { serialNumber = book.SerialNumber }, null);
        }

        [HttpPost("{serialNumber}/category")]
        [SwaggerResponse(201)]
        public async Task<ActionResult> AddCategoriesToBook(long serialNumber, [FromBody, Required] CategoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifies if serialNumber exists
            var book = await _context.Books.FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

            if (book == null)
                return NotFound($"Book with serial number '{serialNumber}' not found");


            // get category in db
            var category = await _context.Categories.FirstOrDefaultAsync(a => a.Name == request.Name);

            // if category doesn't exist it creates a new one
            if (category == null)
            {
                category = new Category { Name = request.Name };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync(); //saves to create category id 
            }

            // check if relationship between book and category already exists, if it exist returns bad request
            var existingRelation = await _context.BookCategories.AnyAsync(ba => ba.SerialNumber == serialNumber && ba.CategoryID == category.ID);

            if (existingRelation)
            {
                ModelState.AddModelError("Categories", $"Category '{request.Name}' is already associated with this book.");
                return BadRequest(ModelState);
            }

            // if relationship doesn't exist it adds a new BookCategory
            var bookCategory = new BookCategory
            {
                SerialNumber = serialNumber,
                CategoryID = category.ID
            };

            _context.BookCategories.AddRange(bookCategory);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetBooks), new { serialNumber = book.SerialNumber }, null);
        }



        [HttpGet("filter")]
        [SwaggerResponse(200, Type = typeof(List<BookResponse>))]
        public async Task<ActionResult<List<BookResponse>>> GetBooks([FromQuery, Required] GetBookRequest filter)
        {
            if (filter.SerialNumber == null && filter.Title == null && filter.Year == null && filter.Publisher == null && filter.Author == null && filter.Category == null)
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
                                               && (filter.Category == null || b.BookCategories.Any(bc => bc.Category.Name.Contains(filter.Category))));       
          
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
        [SwaggerResponse(200, Type = typeof(List<BookResponse>))]
        public async Task<ActionResult<BookResponse>> GetAllBooks()
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



        [HttpPut("{serialNumber}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> UpdateBook(long serialNumber, [FromBody] UpdateBookRequest request)
        {
            // validation
            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits");
            }

            if (request == null)
                return BadRequest("Request body cannot be empty, provide a field to change (Title, Year, Fine per year, Publisher.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //update logic
            var book = await _context.Books.Include(b => b.Publisher)
                                           .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

            if (book == null)
                return NotFound($"Book with Serial Number '{serialNumber}' not found.");
           

            if (request.Title != null)
                book.Title = request.Title;
            

            if (request.Year.HasValue)
                book.Year = request.Year.Value;

            if (request.FinePerDay.HasValue)
                book.FinePerDay = request.FinePerDay.Value;
            

            // check first if new publisher exists on Publisher, if not it creates
            if (request.Publisher != null && book.Publisher.Name != request.Publisher)
            {
                var existingPublisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Name == request.Publisher);

                if (existingPublisher == null)
                {
                    existingPublisher = new Publisher { Name = request.Publisher };
                    _context.Publishers.Add(existingPublisher);
                    await _context.SaveChangesAsync();
                }

                book.PublisherID = existingPublisher.ID;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{serialNumber}/author")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> UpdateBookAuthor(long serialNumber, [FromBody, Required] UpdateBookAuthorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits.");
            }


            // check if book exists
            var book = await _context.Books.Include(b => b.BookAuthors)
                                           .ThenInclude(ba => ba.Author)
                                           .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

            if (book == null)
                return NotFound($"Book with Serial Number '{serialNumber}' not found.");


            // check if old author exists
            var oldAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Name.ToLower() == request.OldAuthorName.ToLower().Trim());

            if (oldAuthor == null)
                return NotFound($"Author '{request.OldAuthorName}' not found.");

            // check if old author is the same as new author
            if (request.OldAuthorName == request.NewAuthorName)
            {
                ModelState.AddModelError("NewAuthorName", "The new author cannot be the same as the old author.");
                return BadRequest(ModelState);
            }

            // checks if new author exists, add it if not
            var newAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Name.ToLower() == request.NewAuthorName.ToLower().Trim());

            if (newAuthor == null)
            {
                newAuthor = new Author { Name = request.NewAuthorName };
                _context.Authors.Add(newAuthor);
                await _context.SaveChangesAsync();
            }

            // Check if BookAuthor relationship exists
            var existingBookAuthor = await _context.BookAuthors.AnyAsync(ba => ba.SerialNumber == book.SerialNumber && ba.AuthorID == oldAuthor.ID);

            if (!existingBookAuthor)
                return BadRequest($"Author '{request.OldAuthorName}' is not associated with this book.");

            // check if new author is already associated with the book
            var isNewAuthorAlreadyAssociated = await _context.BookAuthors.AnyAsync(ba => ba.SerialNumber == serialNumber && ba.AuthorID == newAuthor.ID);

            if (isNewAuthorAlreadyAssociated)
            {
                ModelState.AddModelError("NewAuthorName", $"The author '{request.NewAuthorName}' is already associated with this book.");
                return BadRequest(ModelState);
            }

            // Remove old relationship
            var relationToRemove = await _context.BookAuthors.FirstOrDefaultAsync(ba => ba.SerialNumber == serialNumber && ba.AuthorID == oldAuthor.ID);

            _context.BookAuthors.Remove(relationToRemove);

            // Adds new relationship
            var bookAuthor = new BookAuthor
            {
                SerialNumber = serialNumber,
                AuthorID = newAuthor.ID
            };
            _context.BookAuthors.Add(bookAuthor);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{serialNumber}/category")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> UpdateBookCategory(long serialNumber, [FromBody, Required] UpdateBookCategoryRequest request)
        {
            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // check if book exists
            var book = await _context.Books.Include(b => b.BookCategories)
                                           .ThenInclude(ba => ba.Category)
                                           .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

            if (book == null)
                return NotFound($"Book with Serial Number '{serialNumber}' not found.");


            // check if old category exists
            var oldCategory = await _context.Categories.FirstOrDefaultAsync(a => a.Name.ToLower() == request.OldCategoryName.ToLower().Trim());

            if (oldCategory == null)
                return NotFound($"Category '{request.OldCategoryName}' not found.");

            // check if old category is the same as new author
            if (request.OldCategoryName == request.NewCategoryName)
            {
                ModelState.AddModelError("NewCategoryName", "The new category cannot be the same as the old category.");
                return BadRequest(ModelState);
            }

            // checks if new category exists, add it if not
            var newCategory = await _context.Categories.FirstOrDefaultAsync(a => a.Name.ToLower() == request.NewCategoryName.ToLower().Trim());

            if (newCategory == null)
            {
                newCategory = new Category { Name = request.NewCategoryName };
                _context.Categories.Add(newCategory);
                await _context.SaveChangesAsync();
            }

            // Check if BookCategory relationship exists
            var existingBookCategory = await _context.BookCategories.AnyAsync(ba => ba.SerialNumber == book.SerialNumber && ba.CategoryID == oldCategory.ID);

            if (!existingBookCategory)
                return BadRequest($"Category '{request.OldCategoryName}' is not associated with this book.");

            // check if new category is already associated with the book
            var isNewCategoryAlreadyAssociated = await _context.BookCategories.AnyAsync(ba => ba.SerialNumber == serialNumber && ba.CategoryID == newCategory.ID);

            if (isNewCategoryAlreadyAssociated)
            {
                ModelState.AddModelError("NewCategoryName", $"The category '{request.NewCategoryName}' is already associated with this book.");
                return BadRequest(ModelState);
            }

            // Remove old relationship
            var relationToRemove = await _context.BookCategories.FirstOrDefaultAsync(ba => ba.SerialNumber == serialNumber && ba.CategoryID == oldCategory.ID);

            _context.BookCategories.Remove(relationToRemove);

            // Adds new relationship
            var bookCategory = new BookCategory
            {
                SerialNumber = serialNumber,
                CategoryID = newCategory.ID
            };
            _context.BookCategories.Add(bookCategory);

            await _context.SaveChangesAsync();
            return NoContent();
        }




        [HttpDelete("{serialNumber}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> DeleteBook(long serialNumber)
        {
            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits");
            }

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
            return NoContent();
        }

        [HttpDelete("{serialNumber}/author")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> DeleteBookAuthor(long serialNumber, [FromBody, Required] AuthorRequest authorName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits");
            

            // check if book exists and related author exists
            var book = await _context.Books.Include(b => b.BookAuthors)
                                           .ThenInclude(b => b.Author)
                                           .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

            if (book == null)
                return NotFound($"No book serial number {serialNumber} was found in the database");
            

            var bookAuthor = book.BookAuthors.FirstOrDefault(ba => ba.Author.Name == authorName.Name);

            if (bookAuthor == null)
                return NotFound($"No author named '{authorName.Name}' was found for this book.");
            

            //check how many authors the book has, if only one it can't be deleted
            if (book.BookAuthors.Count() < 2)
                return BadRequest("The book only has one author. Please update it instead of deleting.");
            

            // delete bookAuthor entry
            _context.BookAuthors.Remove(bookAuthor);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{serialNumber}/category")]
        [SwaggerResponse(204)]
        public async Task<IActionResult> DeleteBookCategory(long serialNumber, [FromBody, Required] CategoryRequest categoryName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (serialNumber <= 0 || serialNumber.ToString().Length != 13)
            {
                return BadRequest("SerialNumber must be a positive integer and have exactly 13 digits");
            }

            // check if book exists and related category exists
            var book = await _context.Books.Include(b => b.BookCategories)
                                           .ThenInclude(b => b.Category)
                                           .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

            if (book == null)
            {
                return NotFound($"No book serial number {serialNumber} was found in the database");
            }

            var bookCategory = book.BookCategories.FirstOrDefault(ba => ba.Category.Name == categoryName.Name);

            if (bookCategory == null)
            {
                return NotFound($"No category named '{categoryName.Name}' was found for this book.");
            }

            //check how many categories the book has, if only one it can't be deleted
            if (book.BookCategories.Count() < 2)
            {
                return BadRequest("The book only has one category. Please update it instead of deleting.");
            }

            // delete bookCategory entry
            _context.BookCategories.Remove(bookCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }  
}
