using System.ComponentModel.DataAnnotations;

namespace cloudinvoice_web_ui.DTOs.Clientes
{
    public class InserirClienteDto
{
        public Guid Id { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O NIF é obrigatório.")]
        public string Nif { get; set; }
        [Required(ErrorMessage = "O responsável é obrigatório.")]
        public string Responsavel { get; set; }
        [Required(ErrorMessage = "O email é obrigatório.")]
        public string Email { get; set; }
        
        public int Ativo { get; set; } = 0;

    }
}
