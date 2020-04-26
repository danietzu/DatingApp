using DatingApp.Blazor.Data;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Blazor.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _jSRuntime;
        private readonly string _baseUrl = "https://localhost:4000/api/auth/";
        private static bool _isLoggedIn = false;

        public AuthService(HttpClient http, IJSRuntime JSRuntime)
        {
            _http = http;
            _jSRuntime = JSRuntime;
        }

        [JSInvokable]
        public static void SetAsLoggedIn()
        {
            _isLoggedIn = true;
        }

        public bool CheckIfLoggedIn()
        {
            return _isLoggedIn;
        }

        public async Task Login(LoginForm loginForm)
        {
            var user = JsonSerializer.Serialize(loginForm);
            var response = await _http.PostAsync(_baseUrl + "login",
                                         new StringContent(user,
                                                           Encoding.UTF8,
                                                           "application/json"));
            var content = response.Content.ReadAsStringAsync();
            var result = content.Result;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(result, options);

            await _jSRuntime.InvokeVoidAsync("log", result);
            await _jSRuntime.InvokeVoidAsync("saveToken", authResponse.Token);
        }

        public async Task Logout()
        {
            await _jSRuntime.InvokeVoidAsync("removeToken");
        }
    }
}