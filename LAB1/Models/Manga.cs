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
        public string Genres { get; set; } // Жанри

        [Range(1, 10)]
        public double Rating { get; set; } // Рейтинг

        public string Description { get; set; } // Опис манги

        public string Author { get; set; } // Автор

        public string Illustrator { get; set; } // Ілюстратор

        public int Volumes { get; set; } // Кількість томів

        public int Chapters { get; set; } // Кількість глав

        public string CoverUrl { get; set; } // URL обкладинки манги

        public string Status { get; set; } // Статус завершеності
    }
}
