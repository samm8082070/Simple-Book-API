using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class BookCovers
    {

        [Key] // Mark BookCoverId as the primary key
        public int BookCoverId { get; set; }

        public int BookId { get; set; } // Foreign key

        [Required]
        [MaxLength(255)]
        public String CoverImagePath { get; set; }
    }
}
