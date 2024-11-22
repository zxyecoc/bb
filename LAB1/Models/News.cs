using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAB1.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string Title { get; set; }

        public string NewsText { get; set; } 

        public int AuthorId { get; set; } 
        [ValidateNever]public Author Author { get; set; } 

        public string CoverUrl { get; set; } 

        public List<Comment> Comments { get; set; } = new List<Comment>();

        [NotMapped]
        [ValidateNever] public Rating Rating { get; set; }
        public ICollection<Rating>? Ratings { get; set; } 
        public ICollection<Tag> Tags { get; set; } = new List<Tag>(); 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
