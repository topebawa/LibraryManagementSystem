using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.API.DTOs;
using LibraryManagement.API.Services;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Add a new book
        /// </summary>
        /// <param name="createBookDto">Book creation details</param>
        /// <returns>Created book details</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<BookDto>>> CreateBook(CreateBookDto createBookDto)
        {
            var result = await _bookService.CreateBookAsync(createBookDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetBook), new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// Retrieve all books with optional search and pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <param name="search">Search term for title or author</param>
        /// <returns>Paginated list of books</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<BookDto>>>> GetBooks(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            // Validate pagination parameters
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit maximum page size

            var result = await _bookService.GetAllBooksAsync(pageNumber, pageSize, search);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get a specific book by ID
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>Book details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BookDto>>> GetBook(int id)
        {
            var result = await _bookService.GetBookByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update a book's details
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <param name="updateBookDto">Updated book details</param>
        /// <returns>Updated book details</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<BookDto>>> UpdateBook(int id, UpdateBookDto updateBookDto)
        {
            var result = await _bookService.UpdateBookAsync(id, updateBookDto);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete a book by ID
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}