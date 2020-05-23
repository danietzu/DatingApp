using DatingApp.WASM.Data;
using DatingApp.WASM.Models;
using DatingApp.WASM.Store;
using Fluxor;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.WASM.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private readonly IDispatcher _dispatcher;
        private readonly string _baseUrl;

        public AuthService(HttpClient http,
                           IJSRuntime js,
                           IDispatcher dispatcher)
        {
            _http = http;
            _js = js;
            _dispatcher = dispatcher;
            _baseUrl = "https://localhost:4001/api/" + "auth/";
        }

        public async Task<string> Login(LoginForm loginForm)
        {
            var user = JsonSerializer.Serialize(loginForm);
            var response = await _http.PostAsync(_baseUrl + "login",
                                                 new StringContent(user,
                                                                   Encoding.UTF8,
                                                                   "application/json"));
            var content = response.Content.ReadAsStringAsync();

            if (content.Result.StartsWith("{\"token\""))
            {
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(content.Result,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                await SetCurrentUser(authResponse.User);

                return "OK " + authResponse.Token;
            }

            string errorMessage = ErrorInterceptor.InterceptError(content.Result);

            return errorMessage;
        }

        public async Task SetCurrentUser(User user)
        {
            var userJson = JsonSerializer.Serialize(user);
            await _js.InvokeVoidAsync("saveUser", userJson);

            _dispatcher.Dispatch(new SetCurrentUserAction { User = user });
        }

        public async Task<string> LoggedIn()
        {
            var token = await _js.InvokeAsync<string>("getToken");
            return token;

            // TODO: return bool after token validation
        }

        public async Task<string> Register(RegisterForm registerForm)
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

        public async Task<int> GetLoggedInUserId()
        {
            // temporary solution
            var token = await _js.InvokeAsync<string>("getToken");
            if (string.IsNullOrWhiteSpace(token))
                return 0;

            string secret = "super secret key"; // MUST remove this from here
            var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var jwt = handler.ReadJwtToken(token);
            var id = jwt.Claims.First(claim => claim.Type == "nameid").Value;

            return int.Parse(id);
        }

        public async Task<string> GetLoggedInUsername()
        {
            // temporary solution
            var token = await _js.InvokeAsync<string>("getToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            string secret = "super secret key"; // MUST remove this from here
            var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var jwt = handler.ReadJwtToken(token);
            var name = jwt.Claims.First(claim => claim.Type == "unique_name").Value;

            return name;
        }
    }
}