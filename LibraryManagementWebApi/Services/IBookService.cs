using LibraryManagementWebApi.DTOs;

namespace LibraryManagementWebApi.Services
{
    public interface IBookService
    {
        Task<List<BookResponseDto>> GetBooks(
            string search);

        Task<BookResponseDto> GetBook(int id);

        Task<BookResponseDto> AddBook(
            BookDto dto);

        Task UpdateBook(
            int id,
            BookDto dto);

        Task DeleteBook(int id);
    }
}