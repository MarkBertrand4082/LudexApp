// Andrew Neto
using System.ComponentModel.DataAnnotations;

namespace LudexApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        // Where to go after successful login (used by ToUserPage)
        public string? ReturnUrl { get; set; }
    }
}

