namespace cloudinvoice_web_ui.DTOs.Clientes
{
    public class CustomerProfileDto
{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TradeName { get; set; }
        public string TaxId { get; set; }
        public bool IsActive { get; set; }
        public decimal? CurrentDebt { get; set; }
        public decimal? CreditLimit { get; set; }
        public decimal? TotalInvoiced { get; set; }
        public int? PaymentTermsDays { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public decimal? DefaultDiscount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Responsável Principal
        public string? ContactPersonName { get; set; }
        public string? ContactPersonRole { get; set; }
        public string? ContactPersonEmail { get; set; }
        public string? ContactPersonPhone { get; set; }
    }
}
