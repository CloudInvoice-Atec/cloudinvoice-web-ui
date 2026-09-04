using System.ComponentModel.DataAnnotations;

namespace cloudinvoice_web_ui.DTOs.Identity
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Campo obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Role { get; set; } = "Contabilista";

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}