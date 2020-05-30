using DatingApp.WASM.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.WASM.Services
{
    public class UserService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private readonly AuthService _authService;

        public UserService(HttpClient http,
                           IJSRuntime js,
                           AuthService authService)
        {
            _http = http;
            _js = js;
            _authService = authService;
            _baseUrl = "https://localhost:4001/api/";
        }

        public async Task<PaginatedResult<IEnumerable<User>>> GetUsers(int currentPage,
                                                                       int itemsPerPage,
                                                                       string gender,
                                                                       int minAge,
                                                                       int maxAge,
                                                                       string orderBy,
                                                                       bool? likers,
                                                                       bool? likees)
        {
            var token = await _js.InvokeAsync<string>("getToken");
            if (!_http.DefaultRequestHeaders.Contains("Authorization"))
                _http.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await _http.GetAsync(new Uri(_baseUrl + "users" +
                                                      $"?pageNumber={currentPage}" +
                                                      $"&pageSize={itemsPerPage}" +
                                                      $"&gender={gender}" +
                                                      $"&minAge={minAge}" +
                                                      $"&maxAge={maxAge}" +
                                                      $"&orderBy={orderBy}" +
                                                      $"&likers={likers}" +
                                                      $"&likees={likees}"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var result = DeserializeString<IEnumerable<User>>(content);
                var pagination = JsonSerializer.Deserialize<Pagination>(
                    response.Headers.GetValues("Pagination").FirstOrDefault(),
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                return new PaginatedResult<IEnumerable<User>>
                {
                    Result = result,
                    Pagination = pagination
                };
            }
            else
                return null;
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

        public async Task<HttpResponseMessage> UpdateUser(int id, User user)
        {
            var token = await _js.InvokeAsync<string>("getToken");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userSerialized = JsonSerializer.Serialize(user);
            var stringContent = new StringContent(userSerialized, Encoding.UTF8, "application/json");

            return await _http.PutAsync(_baseUrl + "users/" + id, stringContent);
        }

        public async Task<HttpResponseMessage> SendLike(int recipientId)
        {
            var token = await _js.InvokeAsync<string>("getToken");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var currentUserId = await _authService.GetLoggedInUserId();

            var postResponse = await _http.PostAsync($"{_baseUrl}" +
                                                     $"users/" +
                                                     $"{currentUserId}/" +
                                                     $"like/" +
                                                     $"{recipientId}", null);
            return postResponse;
        }

        public async Task<PaginatedResult<IEnumerable<Message>>> GetMessages(int pageNumber,
                                                                             int itemsPerPage,
                                                                             string messageContainer)
        {
            var token = await _js.InvokeAsync<string>("getToken");
            if (!_http.DefaultRequestHeaders.Contains("Authorization"))
                _http.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var userId = await _authService.GetLoggedInUserId();

            var response = await _http.GetAsync(new Uri(_baseUrl + $"users/{userId}/messages/" +
                                                                   $"?pageNumber={pageNumber}" +
                                                                   $"&pageSize={itemsPerPage}" +
                                                                   $"&messageContainer={messageContainer}"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var result = DeserializeString<IEnumerable<Message>>(content);
                var pagination = JsonSerializer.Deserialize<Pagination>(
                    response.Headers.GetValues("Pagination").FirstOrDefault(),
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                return new PaginatedResult<IEnumerable<Message>>
                {
                    Result = result,
                    Pagination = pagination
                };
            }
            else
                return null;
        }

        public async Task<List<Message>> GetMessageThread(int recipientId)
        {
            var token = await _js.InvokeAsync<string>("getToken");
            if (!_http.DefaultRequestHeaders.Contains("Authorization"))
                _http.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var userId = await _authService.GetLoggedInUserId();

            var response = await _http.GetAsync(new Uri(_baseUrl + $"users/{userId}/messages/" +
                                                                   $"thread/{recipientId}"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return DeserializeString<List<Message>>(content);
            }
            else
                return null;
        }

        public async Task<Message> SendMessage(Message message)
        {
            var token = await _js.InvokeAsync<string>("getToken");
            if (!_http.DefaultRequestHeaders.Contains("Authorization"))
                _http.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var userId = await _authService.GetLoggedInUserId();

            var messageJson = JsonSerializer.Serialize(message);
            var contentToSend = new StringContent(messageJson, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(new Uri(_baseUrl + $"users/{userId}/messages/"),
                                                                    contentToSend);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return DeserializeString<Message>(content);
            }
            else
                return null;
        }

        public async Task<HttpResponseMessage> UploadPhoto(Photo photo)
        {
            var token = await _js.InvokeAsync<string>("getToken");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var multipartContent = new MultipartFormDataContent
            {
                { photo.File, "File", "fileName" }
            };

            var currentUserId = await _authService.GetLoggedInUserId();

            var postResponse = await _http.PostAsync(
                _baseUrl + $"users/{currentUserId}/photos",
                multipartContent);

            return postResponse;
        }

        public async Task<HttpResponseMessage> SetMainPhoto(int photoId)
        {
            var token = await _js.InvokeAsync<string>("getToken");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var currentUserId = await _authService.GetLoggedInUserId();

            var postResponse = await _http.PostAsync(_baseUrl + $"users/{currentUserId}/" +
                                                                $"photos/{photoId}/setMain",
                                                     new StringContent(""));

            return postResponse;
        }

        public async Task<HttpResponseMessage> DeletePhoto(int photoId)
        {
            var token = await _js.InvokeAsync<string>("getToken");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var currentUserId = await _authService.GetLoggedInUserId();

            var postResponse = await _http.DeleteAsync(_baseUrl + $"users/{currentUserId}/" +
                                                                  $"photos/{photoId}");

            return postResponse;
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