namespace LibraryManagementWebApi.DTOs
{
    public class BookResponseDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        public bool IsAvailable { get; set; }
    }
}