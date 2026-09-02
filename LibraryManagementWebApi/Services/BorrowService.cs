using LibraryManagementWebApi.Data;
using LibraryManagementWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementWebApi.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly ApplicationDbContext _context;

        public BorrowService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DateTime> BorrowBook(
            int bookId,
            int userId)
        {
            using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var book =
                    await _context.Books
                        .FirstOrDefaultAsync(
                            x => x.Id == bookId);

                if (book == null)
                {
                    throw new Exception(
                        "Book not found.");
                }

                if (!book.IsAvailable)
                {
                    throw new Exception(
                        "Book is already borrowed.");
                }

                var userExists =
                    await _context.Users
                        .AnyAsync(
                            x => x.Id == userId);

                if (!userExists)
                {
                    throw new Exception(
                        "User not found.");
                }

                var borrowedDate =
                    DateTime.UtcNow;

                var dueDate =
                    borrowedDate.AddDays(14);

                var borrowRecord =
                    new BorrowRecord
                    {
                        BookId = bookId,
                        UserId = userId,
                        BorrowedDate = borrowedDate,
                        DueDate = dueDate,
                        ReturnedDate = null
                    };

                book.IsAvailable = false;

                _context.BorrowRecords.Add(
                    borrowRecord);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return dueDate;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task ReturnBook(
            int bookId,
            int userId)
        {
            using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var borrowRecord =
                    await _context.BorrowRecords
                        .Include(x => x.Book)
                        .FirstOrDefaultAsync(
                            x =>
                                x.BookId == bookId &&
                                x.UserId == userId &&
                                x.ReturnedDate == null);

                if (borrowRecord == null)
                {
                    throw new Exception(
                        "No active borrowing record found.");
                }

                borrowRecord.ReturnedDate =
                    DateTime.UtcNow;

                borrowRecord.Book.IsAvailable = true;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}