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
    [Route("Book")]
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
            if (await _context.Books.AnyAsync(b => b.SerialNumber == request.SerialNumber))
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
                await _context.SaveChangesAsync(); // saves publisher to create id
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

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync(); //saves book to save serial number and bookStock

            // adds bookCopies 'automatically'
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

            _context.BookCopies.AddRange(bookCopiesList);

            // add book Authors
            var bookAuthors = new List<BookAuthor>();

            foreach (var authorRequest in request.Authors)
            {
                var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == authorRequest.Name);
                
                // if author doesn't exist it creates a new one
                if (author == null)
                {
                    author = new Author { Name = authorRequest.Name };
                    _context.Authors.Add(author);
                    await _context.SaveChangesAsync(); //saves to create author id 
                }

                // creates a new BookAuthor relationship
                bookAuthors.Add(new BookAuthor
                {
                    SerialNumber = request.SerialNumber,
                    AuthorID = author.ID
                });
            }
            _context.BookAuthors.AddRange(bookAuthors);

            // add book categories
            var bookCategories = new List<BookCategory>();

            foreach (var categoryRequest in request.Categories)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(a => a.Name == categoryRequest.Name);

                // if category doesn't exist it creates a new one
                if (category == null)
                {
                    category = new Category { Name = categoryRequest.Name };
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync(); //saves to create category id 
                }

                // creates a new BookCategory relationship
                bookCategories.Add(new BookCategory
                {
                    SerialNumber = request.SerialNumber,
                    CategoryID = category.ID
                });
            }
            _context.BookCategories.AddRange(bookCategories);

            await _context.SaveChangesAsync(); // saves BookCopies, BookAuthors, and BookCategories


            return CreatedAtAction(nameof(GetBooks), new { serialNumber = newBook.SerialNumber }, null);
        }


        [HttpGet("filter")]
        [SwaggerResponse(200, Type = typeof(List<BookResponse>))]
        public async Task<ActionResult<List<BookResponse>>> GetBooks([FromQuery, Required] GetBookRequest filter)
        {
            if (filter.SerialNumber == null && filter.Title == null && filter.Year == null && filter.Publisher == null && filter.Author == null && filter.Category == null)
            {
                return BadRequest("At least one filter (Serial Number, Title, Year, Publisher, Author, or Category) must be provided.");
            }


            var books =await _context.Books.Where(b => (filter.SerialNumber == null || b.SerialNumber == filter.SerialNumber)
                                               && (filter.Title == null || b.Title.Contains(filter.Title))
                                               && (filter.Year == null || b.Year == filter.Year)
                                               && (filter.Publisher == null || b.Publisher.Name.Contains(filter.Publisher))
                                               && (filter.Author == null || b.BookAuthors.Any(ba => ba.Author.Name.Contains(filter.Author)))
                                               && (filter.Category == null || b.BookCategories.Any(bc => bc.Category.Name.Contains(filter.Category))))
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

            return Ok(books);
        }

        [HttpGet("list")]
        [SwaggerResponse(200, Type = typeof(List<BookResponse>))]
        public async Task<ActionResult<BookResponse>> GetAllBooks()
        {
            var books = await _context.Books
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

            return Ok(books);
        }


        [HttpPut("{serialNumber}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> UpdateBook(long serialNumber, [FromBody, Required] UpdateBookRequest request)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

            if (book == null)
                return NotFound($"Book with Serial Number '{serialNumber}' not found.");

            // DTO validation
            if (request == null)
                return BadRequest("Request body cannot be empty, provide a field to change (Title, Year, Fine per year, Publisher, Authors, Categories).");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //update logic
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

            // update authors
            if (request.Authors != null)
            {
                var currentAuthors = book.BookAuthors.Select(ba => ba.Author).ToList();

                foreach (var authorRequest in request.Authors)
                {
                    var newAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Name.ToLower() == authorRequest.Name.ToLower().Trim());

                    // create author if it doesn't exist
                    if (newAuthor == null)
                    {
                        newAuthor = new Author { Name = authorRequest.Name };
                        _context.Authors.Add(newAuthor);
                        await _context.SaveChangesAsync(); //saves here to create author ID
                    }

                    // check if author has been already associated with book, if not it creates a new one, if it already exists doesn't do anything
                    var existingAuthorRelation = currentAuthors.FirstOrDefault(a => a.Name.ToLower() == authorRequest.Name.ToLower().Trim());

                    if (existingAuthorRelation == null)
                    {
                        _context.BookAuthors.Add(new BookAuthor
                        {
                            SerialNumber = serialNumber,
                            AuthorID = newAuthor.ID
                        });
                    }
                    // removes the author from the current authors list, so later it isn't removed
                    else
                    {
                        currentAuthors.Remove(existingAuthorRelation);
                    }
                }

                // removes any author that are still in the current list
                foreach (var oldAuthor in currentAuthors)
                {
                    var relationToRemove = await _context.BookAuthors.FirstOrDefaultAsync(ba => ba.SerialNumber == serialNumber && ba.AuthorID == oldAuthor.ID);
                    if (relationToRemove != null)
                    {
                        _context.BookAuthors.Remove(relationToRemove);
                    }
                }
            }

            // update categpries
            if (request.Categories != null)
            {
                var currentCategories = book.BookCategories.Select(bc => bc.Category).ToList();

                foreach (var categoryRequest in request.Categories)
                {
                    var newCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == categoryRequest.Name.ToLower().Trim());

                    // create category if it doesn't exist
                    if (newCategory == null)
                    {
                        newCategory = new Category { Name = categoryRequest.Name };
                        _context.Categories.Add(newCategory);
                        await _context.SaveChangesAsync(); //saves here to create category ID
                    }

                    // check if category has been already associated with book, if not it creates a new one, if it already exists doesn't do anything
                    var existingCategoryRelation = currentCategories.FirstOrDefault(a => a.Name.ToLower() == categoryRequest.Name.ToLower().Trim());

                    if (existingCategoryRelation == null)
                    {
                        _context.BookCategories.Add(new BookCategory
                        {
                            SerialNumber = serialNumber,
                            CategoryID = newCategory.ID
                        });
                    }
                    // removes the category from the current categories list, so later it isn't removed
                    else
                    {
                        currentCategories.Remove(existingCategoryRelation);
                    }
                }

                // removes any category that are still in the current list
                foreach (var oldCategory in currentCategories)
                {
                    var relationToRemove = await _context.BookCategories.FirstOrDefaultAsync(ba => ba.SerialNumber == serialNumber && ba.CategoryID == oldCategory.ID);
                    if (relationToRemove != null)
                    {
                        _context.BookCategories.Remove(relationToRemove);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{serialNumber}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> DeleteBook(long serialNumber)
        {
            // check if book exists
            var book = await _context.Books.FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);

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
    }  
}
