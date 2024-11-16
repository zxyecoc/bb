using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; } // Унікальний ідентифікатор коментаря

        [Required]
        public string Content { get; set; } // Текст коментаря

        public string UserName { get; set; } // Ім'я користувача

        public User user { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Дата і час створення коментаря

        public Manga Manga { get; set; }
        [Required]
        public int MangaId { get; set; } // Зв'язок з мангою 
    }

}
