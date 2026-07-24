namespace cloudinvoice_web_ui.Models.Auth
{
    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }
}
