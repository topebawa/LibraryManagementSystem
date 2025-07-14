using Microsoft.EntityFrameworkCore;
using AutoMapper;
using LibraryManagement.API.DTOs;
using LibraryManagement.API.Models;

namespace LibraryManagement.API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Book mappings
            CreateMap<Book, BookDto>();
            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();

            // User mappings
            CreateMap<User, RegisterDto>().ReverseMap();
        }
    }
}