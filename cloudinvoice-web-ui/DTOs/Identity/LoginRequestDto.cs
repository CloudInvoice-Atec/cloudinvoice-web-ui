using System.ComponentModel.DataAnnotations;

namespace cloudinvoice_web_ui.DTOs.Identity
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Campo obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Password { get; set; } = string.Empty;
    }
}