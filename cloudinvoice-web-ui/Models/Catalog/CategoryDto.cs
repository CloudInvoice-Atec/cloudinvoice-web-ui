namespace cloudinvoice_web_ui.Models.Catalog
{
    // DTO para a listagem de categorias na dropdown
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}