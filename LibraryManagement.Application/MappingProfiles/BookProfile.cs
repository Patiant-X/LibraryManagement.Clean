using AutoMapper;
using LibraryManagement.Application.Features.Books.Commands.CreateBook;
using LibraryManagement.Application.Features.Books.Commands.UpdateBook;
using LibraryManagement.Application.Features.Books.Queries.GetAllBooks;
using LibraryManagement.Application.Features.Books.Queries.GetBookDetails;
using LibraryManagement.Application.Features.Books.Queries.GetBooksBySearch;
using LibraryManagement.Domain;

namespace LibraryManagement.Application.MappingProfiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookDto, Book>().ReverseMap();
            CreateMap<CreateBookCommand, Book>();
            CreateMap<UpdateBookCommand, Book>();

            CreateMap<Book, BookDetailsDto>();
            CreateMap<Book, BookSearchDto>();

            CreateMap<Book, Features.Books.Commands.CreateBook.CreateBookDto>();
            CreateMap<Book, UpdateBookDto>();
        }
    }
}
