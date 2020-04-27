using DatingApp.Blazor.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Blazor.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl = "https://localhost:4000/api/auth/";

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> Login(LoginForm loginForm)
        {
            var user = JsonSerializer.Serialize(loginForm);
            var response = await _http.PostAsync(_baseUrl + "login",
                                         new StringContent(user,
                                                           Encoding.UTF8,
                                                           "application/json"));
            var content = response.Content.ReadAsStringAsync();

            return content.Result;
        }

        public async Task<string> Register(LoginForm registerForm)
        {
            var user = JsonSerializer.Serialize(registerForm);
            var response = await _http.PostAsync(_baseUrl + "register",
                                         new StringContent(user,
                                                           Encoding.UTF8,
                                                           "application/json"));
            var content = response.Content.ReadAsStringAsync();

            return content.Result;
        }
    }
}