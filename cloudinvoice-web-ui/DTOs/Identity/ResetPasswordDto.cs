using System.ComponentModel.DataAnnotations;

namespace cloudinvoice_web_ui.DTOs.Identity
{
    public class ResetPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required, MinLength(6)]
        public string NewPassword { get; set; }
    }
}
