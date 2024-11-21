using System.ComponentModel.DataAnnotations;
namespace LAB1.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль і підтвердження пароля не співпадають.")]
        public string ConfirmPassword { get; set; }
    }
}
