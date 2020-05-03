using DatingApp.Blazor.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Blazor.Services
{
    public class UserService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _http;
        private readonly IConfiguration _configuration;
        private readonly IJSRuntime _js;

        public UserService(HttpClient http,
                           IConfiguration configuration,
                           IJSRuntime js)
        {
            _http = http;
            _configuration = configuration;
            _js = js;
            _baseUrl = _configuration.GetSection("ApiUrl").Value;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var token = await _js.InvokeAsync<string>("getToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var response = await SendHttpRequestAsync(new Uri(_baseUrl + "users"),
                                                      HttpMethod.Get,
                                                      token);
            return DeserializeString<IEnumerable<User>>(response);
        }

        public async Task<User> GetUser(int id)
        {
            var token = await _js.InvokeAsync<string>("getToken");
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var response = await SendHttpRequestAsync(new Uri(_baseUrl + "users/" + id),
                                                      HttpMethod.Get,
                                                      token);
            return DeserializeString<User>(response);
        }

        private async Task<string> SendHttpRequestAsync(Uri uri,
                                                        HttpMethod method,
                                                        string token)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = method
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            request.Headers.Add("Authorization", "Bearer " + token);

            var response = await _http.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        private T DeserializeString<T>(string content)
        {
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}