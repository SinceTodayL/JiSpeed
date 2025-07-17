namespace JISpeed.Api.Models
{
    public class LoginRequest
    {
        public string Id { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}