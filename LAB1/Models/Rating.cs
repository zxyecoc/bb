using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; } // Унікальний ідентифікатор оцінки

        public string UserName { get; set; }  // Замість UserId зберігаємо UserName

        public News News { get; set; }
        [Required]
        public int NewsId { get; set; } // Ідентифікатор манги

        [Range(1, 10)]
        public int UserRating { get; set; } // Оцінка користувача (1-10)
        public DateTime CreatedAt { get; set; }

    }
}
