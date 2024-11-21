using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAB1.Models
{
    public class Manga
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string Title { get; set; }

        [Required]
        public int ReleaseYear { get; set; } 
        
        [Required]
        [ValidateNever]public string Genres { get; set; } = "Unknown"; 

        public string Description { get; set; } 

        public int AuthorId { get; set; } 
        [ValidateNever]public Author Author { get; set; } 

        public int IllustratorId { get; set; } 
        [ValidateNever]public Author Illustrator { get; set; } 

        public int Volumes { get; set; } 

        public int Chapters { get; set; } 

        public string CoverUrl { get; set; } 

        public bool Status { get; set; } 

        public List<Comment> Comments { get; set; } = new List<Comment>();

        [NotMapped]
        [ValidateNever] public Rating Rating { get; set; }
        public ICollection<Rating>? Ratings { get; set; } 
        public double? AverageRating { get; set; }

        public ICollection<Tag> Tags { get; set; } = new List<Tag>(); 
        public ICollection<Chapter> Chapter { get; set; } = new List<Chapter>();
    }
}
