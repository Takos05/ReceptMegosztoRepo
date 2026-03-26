using System.ComponentModel.DataAnnotations;

namespace Receptek.Front.MODEL
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "A felhasználónév megadása kötelező.")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Az email megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Érvényes email címet adj meg.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        public string Password { get; set; } = "";
    }
}
