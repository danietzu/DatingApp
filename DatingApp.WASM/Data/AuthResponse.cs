using DatingApp.WASM.Models;

namespace DatingApp.WASM.Data
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public User User { get; set; }
    }
}