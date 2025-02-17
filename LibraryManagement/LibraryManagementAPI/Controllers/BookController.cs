using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Azure;
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
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [SwaggerResponse(200, Type = typeof(BookResponse))]
        public async Task<IActionResult> CreateBooks([FromBody, Required] IEnumerable<BookRequest> books)
        {
            if (books == null || !books.Any())
            {
                return BadRequest("At least one book must be provided.");
            }

            try
            {
                // load data present in db
                var authorsInDb = await _context.Authors.Select(a => a.Name).ToListAsync();
                var categoriesInDb = await _context.Categories.Select(c => c.Name).ToListAsync();
                var publishersInDb = await _context.Publishers.Select(p => p.Name).ToListAsync();

                var newAuthors = new List<Author>();
                var newCategories = new List<Category>();
                var newPublishers = new List<Publisher>();
                var booksList = new List<Book>();
                var bookCopiesList = new List<BookCopy>();


                foreach (var bookRequest in books)
                {
                    //check if book already exists through sn
                    if (_context.Books.Any(b => b.SerialNumber == bookRequest.SerialNumber))
                    {
                        throw new MyException($"Book with the serial number '{bookRequest.SerialNumber}' already exists");
                    }

                    // checks if authors exist and add new ones to list
                    foreach (var authorName in bookRequest.AuthorName)
                    {
                        if (authorName.Length < 1 || authorName.Length > 30)
                        {
                            return BadRequest($"Author name '{authorName}' is invalid. It must be between 1 and 30 characters.");
                        }

                        if (!Regex.IsMatch(authorName, @"^(?!.*\b(?i)(drop|delete|insert|update|select|alter|table)\b)([\p{L}\s\.\-']+)$"))
                        {
                            return BadRequest($"Author name '{authorName}' contains invalid characters or forbidden words.");
                        }

                        if (!authorsInDb.Contains(authorName))
                        {
                            newAuthors.Add(new Author { Name = authorName });
                        }
                    }

                    // checks if categories exist and add new ones to list
                    foreach (var categoryName in bookRequest.CategoryName)
                    {
                        if (categoryName.Length < 1 || categoryName.Length > 30)
                        {
                            return BadRequest($"Category name '{categoryName}' is invalid. It must be between 1 and 30 characters.");
                        }

                        if (!Regex.IsMatch(categoryName, @"^(?!.*\b(?i)(drop|delete|insert|update|select|alter|table)\b)([\p{L}\s]+)$"))
                        {
                            return BadRequest($"Category name '{categoryName}' contains invalid characters or forbidden words.");
                        }

                        if (!categoriesInDb.Contains(categoryName))
                        {
                            newCategories.Add(new Category { Name = categoryName });
                        }
                    }

                    // checks if publisher exist and add new ones to list
                    if (!publishersInDb.Contains(bookRequest.PublisherName))
                    {
                        newPublishers.Add(new Publisher { Name = bookRequest.PublisherName });
                    }
                }

                // adds new entries in the db
                _context.Authors.AddRange(newAuthors);
                _context.Categories.AddRange(newCategories);
                _context.Publishers.AddRange(newPublishers);
                await _context.SaveChangesAsync();


                foreach (var bookRequest in books)
                {
                    // Creating new books
                    var publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Name == bookRequest.PublisherName);

                    var newBook = new Book
                    {
                        SerialNumber = bookRequest.SerialNumber,
                        Title = bookRequest.Title,
                        Year = bookRequest.Year,
                        FinePerDay = bookRequest.FinePerDay,
                        Publisher = publisher,
                        BookStock = new BookStock
                        {
                            TotalAmount = bookRequest.TotalAmount,
                            AvailableAmount = bookRequest.TotalAmount
                        }
                    };

                    booksList.Add(newBook);

                    // adding bookCopies
                    for (int i = 0; i < bookRequest.TotalAmount; i++)
                    {
                        bookCopiesList.Add(new BookCopy
                        {
                            SerialNumber = bookRequest.SerialNumber,
                            BookConditionID = 1,
                        });
                    }
                }

                await _context.Books.AddRangeAsync(booksList);
                await _context.SaveChangesAsync();
                await _context.BookCopies.AddRangeAsync(bookCopiesList);
                await _context.SaveChangesAsync();

                //adds the relationships between books and authors/categories (BookAuthor and BookCategory)
                foreach (var bookRequest in books)
                {
                    foreach (var author in bookRequest.AuthorName)
                    {
                        var authorInAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Name == author);

                        _context.BookAuthors.Add(new BookAuthor
                        {
                            SerialNumber = bookRequest.SerialNumber,
                            AuthorID = authorInAuthor.ID
                        });
                    }

                    foreach (var category in bookRequest.CategoryName)
                    {
                        var categoryInCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category);

                        _context.BookCategories.Add(new BookCategory
                        {
                            SerialNumber = bookRequest.SerialNumber,
                            CategoryID = categoryInCategory.ID
                        });
                    }
                }

                await _context.SaveChangesAsync();


                var response = booksList.Select(b => new BookResponse
                {
                    SerialNumber = b.SerialNumber,
                    Title = b.Title,
                    Year = b.Year,
                    FinePerDay = b.FinePerDay,
                    PublisherName = b.Publisher.Name,
                    AuthorName = b.BookAuthors.Select(ba => ba.Author.Name).ToList(),
                    CategoryName = b.BookCategories.Select(bc => bc.Category.Name).ToList(),
                    TotalAmount = b.BookStock.TotalAmount,
                    AvailableAmount = b.BookStock.TotalAmount
                });

                return Ok(response);
            }
            catch (MyException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [SwaggerResponse(200, Type = typeof(BookResponse))]
        public async Task<IActionResult> GetBooks([FromQuery] BookQueryRequest filter)
        {
            if (filter.SerialNumber == null && filter.Title == null && filter.Year == null && filter.Publisher == null && filter.Author == null && filter.Category == null)    
            {
                return BadRequest("At least one filter (Serial Number, Title, Year, Publisher, Author, or Category) must be provided.");
            }

            try
            {
                // did LINQ through query method, due to the complexy of what I wanted (being more similar to sql)
                var query = from book in _context.Books
                            join bookAuthor in _context.BookAuthors on book.SerialNumber equals bookAuthor.SerialNumber
                            join author in _context.Authors on bookAuthor.AuthorID equals author.ID
                            join bookCategory in _context.BookCategories on book.SerialNumber equals bookCategory.SerialNumber
                            join category in _context.Categories on bookCategory.CategoryID equals category.ID
                            join publisher in _context.Publishers on book.PublisherID equals publisher.ID
                            join bookStock in _context.BookStocks on book.SerialNumber equals bookStock.SerialNumber
                            where (filter.SerialNumber == null || book.SerialNumber == filter.SerialNumber)
                               && (filter.Title == null || book.Title.Contains(filter.Title))
                               && (filter.Year == null || book.Year == filter.Year)
                               && (filter.Publisher == null || publisher.Name.Contains(filter.Publisher))
                               && (filter.Author == null || author.Name.Contains(filter.Author))
                               && (filter.Category == null || category.Name.Contains(filter.Category))
                            group new { book, author, category, publisher, bookStock } by new
                            {
                                book.SerialNumber,
                                book.Title,
                                book.Year,
                                book.FinePerDay,
                                publisher.Name,
                                bookStock.TotalAmount,
                                bookStock.AvailableAmount
                            } into grouped
                            select new BookResponse
                            {
                                SerialNumber = grouped.Key.SerialNumber,
                                Title = grouped.Key.Title,
                                Year = grouped.Key.Year,
                                FinePerDay = grouped.Key.FinePerDay,
                                PublisherName = grouped.Key.Name,
                                AuthorName = grouped.Select(g => g.author.Name).Distinct().ToList(),
                                CategoryName = grouped.Select(g => g.category.Name).Distinct().ToList(),
                                TotalAmount = grouped.Key.TotalAmount,
                                AvailableAmount = grouped.Key.AvailableAmount
                            };

                var result = await query.ToListAsync();

                if (result.IsNullOrEmpty())
                    return NotFound("Book not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("books-list")]
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
                
    }
}
