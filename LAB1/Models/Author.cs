using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; } // Унікальний ідентифікатор автора

        [Required]
        public string? Name { get; set; } // Ім'я автора
    }
}
