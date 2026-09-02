using System.ComponentModel.DataAnnotations;

namespace LibraryManagementMVC.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(150)]
        public string Author { get; set; }

        [Required]
        [StringLength(100)]
        public string Genre { get; set; }

        public bool IsAvailable { get; set; }
    }
}