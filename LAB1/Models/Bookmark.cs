using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class Bookmark
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } // Посилання на користувача
        public User User { get; set; } // Навігаційна властивість для користувача
        public int MangaId { get; set; } // Посилання на мангу
        public Manga Manga { get; set; } // Навігаційна властивість для манги
    }
}
