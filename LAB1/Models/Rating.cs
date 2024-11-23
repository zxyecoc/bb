using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class Likes
    {
        [Key]
        public int Id { get; set; } // Унікальний ідентифікатор лайка

        [Required]
        public string UserName { get; set; }  // Ім'я користувача, який поставив лайк

        [Required]
        public int NewsId { get; set; } // Ідентифікатор новини

        public News News { get; set; } // Навігаційна властивість до новини

    }
}
