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
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        [NotMapped]
        public int LikeCount { get; set; } // Кількість лайків

        public ICollection<Likes> Likes { get; set; } = new List<Likes>(); 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
