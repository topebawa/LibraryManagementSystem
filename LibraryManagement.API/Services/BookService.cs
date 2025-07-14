using Microsoft.EntityFrameworkCore;
using AutoMapper;
using LibraryManagement.API.Data;
using LibraryManagement.API.DTOs;
using LibraryManagement.API.Models;

namespace LibraryManagement.API.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<BookDto>> CreateBookAsync(CreateBookDto createBookDto)
        {
            try
            {
                // Check if ISBN already exists
                var existingBook = await _context.Books
                    .FirstOrDefaultAsync(b => b.ISBN == createBookDto.ISBN);

                if (existingBook != null)
                {
                    return new ApiResponse<BookDto>
                    {
                        Success = false,
                        Message = "A book with this ISBN already exists",
                        Errors = new List<string> { "ISBN must be unique" }
                    };
                }

                var book = _mapper.Map<Book>(createBookDto);
                book.CreatedAt = DateTime.UtcNow;
                book.UpdatedAt = DateTime.UtcNow;

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                var bookDto = _mapper.Map<BookDto>(book);

                return new ApiResponse<BookDto>
                {
                    Success = true,
                    Message = "Book created successfully",
                    Data = bookDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<BookDto>
                {
                    Success = false,
                    Message = "Failed to create book",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PaginatedResponse<BookDto>>> GetAllBooksAsync(int pageNumber, int pageSize, string? search)
        {
            try
            {
                var query = _context.Books.AsQueryable();

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var books = await query
                    .OrderBy(b => b.Title)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var bookDtos = _mapper.Map<List<BookDto>>(books);

                var paginatedResponse = new PaginatedResponse<BookDto>
                {
                    Data = bookDtos,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return new ApiResponse<PaginatedResponse<BookDto>>
                {
                    Success = true,
                    Message = "Books retrieved successfully",
                    Data = paginatedResponse
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<BookDto>>
                {
                    Success = false,
                    Message = "Failed to retrieve books",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<BookDto>> GetBookByIdAsync(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    return new ApiResponse<BookDto>
                    {
                        Success = false,
                        Message = "Book not found",
                        Errors = new List<string> { $"Book with ID {id} does not exist" }
                    };
                }

                var bookDto = _mapper.Map<BookDto>(book);

                return new ApiResponse<BookDto>
                {
                    Success = true,
                    Message = "Book retrieved successfully",
                    Data = bookDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<BookDto>
                {
                    Success = false,
                    Message = "Failed to retrieve book",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<BookDto>> UpdateBookAsync(int id, UpdateBookDto updateBookDto)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    return new ApiResponse<BookDto>
                    {
                        Success = false,
                        Message = "Book not found",
                        Errors = new List<string> { $"Book with ID {id} does not exist" }
                    };
                }

                // Check if ISBN already exists for a different book
                var existingBook = await _context.Books
                    .FirstOrDefaultAsync(b => b.ISBN == updateBookDto.ISBN && b.Id != id);

                if (existingBook != null)
                {
                    return new ApiResponse<BookDto>
                    {
                        Success = false,
                        Message = "A book with this ISBN already exists",
                        Errors = new List<string> { "ISBN must be unique" }
                    };
                }

                // Update book properties
                book.Title = updateBookDto.Title;
                book.Author = updateBookDto.Author;
                book.ISBN = updateBookDto.ISBN;
                book.PublishedDate = updateBookDto.PublishedDate;
                book.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var bookDto = _mapper.Map<BookDto>(book);

                return new ApiResponse<BookDto>
                {
                    Success = true,
                    Message = "Book updated successfully",
                    Data = bookDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<BookDto>
                {
                    Success = false,
                    Message = "Failed to update book",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteBookAsync(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Book not found",
                        Errors = new List<string> { $"Book with ID {id} does not exist" }
                    };
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Book deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to delete book",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}