using DatingApp.Blazor.Data;
using Microsoft.JSInterop;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Blazor.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private readonly string _baseUrl = "https://localhost:4000/api/auth/";

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task<string> Login(LoginForm loginForm)
        {
            var user = JsonSerializer.Serialize(loginForm);
            var response = await _http.PostAsync(_baseUrl + "login",
                                                 new StringContent(user,
                                                                   Encoding.UTF8,
                                                                   "application/json"));
            var content = response.Content.ReadAsStringAsync();

            string errorMessage = ErrorInterceptor.InterceptError(content.Result);

            return errorMessage;
        }

        public async Task<string> Register(LoginForm registerForm)
        {
            var user = JsonSerializer.Serialize(registerForm);
            var response = await _http.PostAsync(_baseUrl + "register",
                                                 new StringContent(user,
                                                                   Encoding.UTF8,
                                                                   "application/json"));
            var content = response.Content.ReadAsStringAsync();

            string errorMessage = ErrorInterceptor.InterceptError(content.Result);

            return errorMessage;
        }
    }
}