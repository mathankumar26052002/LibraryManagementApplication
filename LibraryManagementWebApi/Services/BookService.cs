using LibraryManagementWebApi.Data;
using LibraryManagementWebApi.DTOs;
using LibraryManagementWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementWebApi.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookResponseDto>> GetBooks(
            string search)
        {
            var query =
                _context.Books
                    .AsNoTracking()
                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.Title.Contains(search) ||
                    x.Author.Contains(search) ||
                    x.Genre.Contains(search));
            }

            return await query
                .OrderBy(x => x.Title)
                .Select(x => new BookResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Author = x.Author,
                    Genre = x.Genre,
                    IsAvailable = x.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<BookResponseDto> GetBook(
            int id)
        {
            var book =
                await _context.Books
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == id);

            if (book == null)
            {
                throw new Exception(
                    "Book not found.");
            }

            return new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                IsAvailable = book.IsAvailable
            };
        }

        public async Task<BookResponseDto> AddBook(
            BookDto dto)
        {
            var title = dto.Title.Trim();
            var author = dto.Author.Trim();

            var duplicate =
                await _context.Books.AnyAsync(
                    x =>
                        x.Title == title &&
                        x.Author == author);

            if (duplicate)
            {
                throw new Exception(
                    "Book already exists.");
            }

            var book = new Book
            {
                Title = title,
                Author = author,
                Genre = dto.Genre.Trim(),
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Books.Add(book);

            await _context.SaveChangesAsync();

            return new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                IsAvailable = book.IsAvailable
            };
        }

        public async Task UpdateBook(
            int id,
            BookDto dto)
        {
            var book =
                await _context.Books
                    .FirstOrDefaultAsync(
                        x => x.Id == id);

            if (book == null)
            {
                throw new Exception(
                    "Book not found.");
            }

            var title = dto.Title.Trim();
            var author = dto.Author.Trim();

            var duplicate =
                await _context.Books.AnyAsync(
                    x =>
                        x.Id != id &&
                        x.Title == title &&
                        x.Author == author);

            if (duplicate)
            {
                throw new Exception(
                    "Another book with the same title and author already exists.");
            }

            book.Title = title;
            book.Author = author;
            book.Genre = dto.Genre.Trim();

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBook(int id)
        {
            var book =
                await _context.Books
                    .FirstOrDefaultAsync(
                        x => x.Id == id);

            if (book == null)
            {
                throw new Exception(
                    "Book not found.");
            }

            if (!book.IsAvailable)
            {
                throw new Exception(
                    "Cannot delete a borrowed book.");
            }

            _context.Books.Remove(book);

            await _context.SaveChangesAsync();
        }
    }
}