using System.ComponentModel.DataAnnotations;

namespace LibraryManagementWebApi.DTOs
{
    public class BookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(150)]
        public string Author { get; set; }

        [Required]
        [StringLength(100)]
        public string Genre { get; set; }
    }
}