using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; } // Унікальний ідентифікатор тегу

        [Required]
        public string Name { get; set; } // Назва тегу

        public ICollection<News> News { get; set; } = new List<News>(); // Зв'язок з мангами

    }
}
