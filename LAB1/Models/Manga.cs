using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class Manga
    {
        [Key]
        public int Id { get; set; } // Унікальний ідентифікатор манги

        [Required]
        public string Title { get; set; } // Назва манги

        [Required]
        public int ReleaseYear { get; set; } // Рік виходу
        
        [Required]
        [ValidateNever]public string Genres { get; set; } = "Unknown"; // Жанри

        public string Description { get; set; } // Опис манги

        public int AuthorId { get; set; } // Зовнішній ключ
        [ValidateNever]public Author Author { get; set; } // Навігаційна властивість

        public int IllustratorId { get; set; } // Зовнішній ключ
        [ValidateNever]public Author Illustrator { get; set; } // Ілюстратор

        public int Volumes { get; set; } // Кількість томів

        public int Chapters { get; set; } // Кількість глав

        public string CoverUrl { get; set; } // URL обкладинки манги

        public bool Status { get; set; } // True - завершено, False - продовжується

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Rating>? Ratings { get; set; }
        public double? AverageRating { get; set; }

        public ICollection<Tag> Tags { get; set; } = new List<Tag>(); // Зв'язок з тегами

    }
}
