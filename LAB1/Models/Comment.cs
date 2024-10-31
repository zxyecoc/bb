using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; } // Унікальний ідентифікатор коментаря

        public string Content { get; set; } // Текст коментаря

        public string User { get; set; } // Ім'я користувача

        public DateTime CreatedAt { get; set; } // Дата і час створення коментаря
    }
}
