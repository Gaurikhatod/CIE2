using System.ComponentModel.DataAnnotations;

namespace CourseEnrollment.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public string Duration { get; set; } = string.Empty;

        [Range(0, 10000000)]
        public decimal Fees { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }
    }
}