using DatingApp.Blazor.Models;

namespace DatingApp.Blazor.Data
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public User User { get; set; }
    }
}