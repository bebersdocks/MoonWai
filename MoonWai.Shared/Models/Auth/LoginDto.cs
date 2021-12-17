namespace MoonWai.Shared.Models.Auth
{
    public class LoginDto
    {        
        public string Username { get; set; }
        public string Password { get; set; }
        public bool   Trusted  { get; set; } = false;
    }
}
