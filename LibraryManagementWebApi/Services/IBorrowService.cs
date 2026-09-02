namespace LibraryManagementWebApi.Services
{
    public interface IBorrowService
    {
        Task<DateTime> BorrowBook(
            int bookId,
            int userId);

        Task ReturnBook(
            int bookId,
            int userId);
    }
}