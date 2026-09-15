using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace cloudinvoice_web_ui.DTOs.Configuracoes
{
    public class EmpresaDto
    {
        
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O NIF é obrigatório.")]
        public string TaxNumber { get; set; }

        public string PrimaryActivityCode { get; set; }

        
        [Required(ErrorMessage = "A morada é obrigatória.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "O código postal é obrigatório.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "A localidade é obrigatória.")]
        public string City { get; set; }

        public Guid CountryId { get; set; }

        
        public IBrowserFile Logo { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "Telefone inválido.")]
        public string Phone { get; set; }

        [Url(ErrorMessage = "Website inválido.")]
        public string Website { get; set; }

        
        public string RegistryOffice { get; set; }
        public string CommercialRegistrationNumber { get; set; }

        [Required(ErrorMessage = "O capital social é obrigatório.")]
        public decimal ShareCapital { get; set; }

        
        [Required(ErrorMessage = "O nome do banco é obrigatório.")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "O IBAN é obrigatório.")]
        [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z0-9]{1,30}$", ErrorMessage = "IBAN inválido.")]
        public string Iban { get; set; }

        [Required(ErrorMessage = "O SWIFT é obrigatório.")]
        [RegularExpression(@"^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$", ErrorMessage = "SWIFT inválido.")]
        public string Swift { get; set; }
    }
}
