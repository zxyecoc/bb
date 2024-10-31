using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class Bookmark
    {
        [Key]
        public int Id { get; set; } // Унікальний ідентифікатор закладки

        public int MangaId { get; set; } // Ідентифікатор манги (може бути просто числовим значенням)

        public string User { get; set; } // Ім'я користувача, який створив закладку
    }
}
