using System.ComponentModel.DataAnnotations;

namespace cloudinvoice_web_ui.DTOs.Clientes
{
    public class InserirClienteDto
{
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "O NIF é obrigatório.")]
        public string TaxNumber { get; set; }
        [Required(ErrorMessage = "O responsável é obrigatório.")]
        public string ContactPersonName { get; set; }
        [Required(ErrorMessage = "O email é obrigatório.")]
        public string ContactPersonEmail { get; set; }
        public bool IsActive { get; set; } = true;

    }   
}
