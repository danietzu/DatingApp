using System.ComponentModel.DataAnnotations;

namespace DatingApp.Blazor.Data
{
    public class LoginForm
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}