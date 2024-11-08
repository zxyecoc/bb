using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; } // Унікальний ідентифікатор тегу

        [Required]
        public string Name { get; set; } // Назва тегу

        public ICollection<Manga> Mangas { get; set; } = new List<Manga>(); // Зв'язок з мангами

    }
}
