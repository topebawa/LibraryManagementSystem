using Microsoft.EntityFrameworkCore;
using LibraryManagement.API.DTOs;

namespace LibraryManagement.API.Services
{
    public interface IBookService
    {
        Task<ApiResponse<BookDto>> CreateBookAsync(CreateBookDto createBookDto);
        Task<ApiResponse<PaginatedResponse<BookDto>>> GetAllBooksAsync(int pageNumber, int pageSize, string? search);
        Task<ApiResponse<BookDto>> GetBookByIdAsync(int id);
        Task<ApiResponse<BookDto>> UpdateBookAsync(int id, UpdateBookDto updateBookDto);
        Task<ApiResponse<bool>> DeleteBookAsync(int id);
    }
}