using DatingApp.WASM.Data;
using System.Text.Json;

namespace DatingApp.WASM.Services
{
    public class ErrorInterceptor
    {
        public static string InterceptError(string httpResult)
        {
            string errorMessage;
            if (httpResult.StartsWith("{"))
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(httpResult,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                if (errorResponse.Errors != null)
                {
                    errorMessage = $"{errorResponse.Title}" + "\n";

                    if (errorResponse.Errors.Username != null)
                        foreach (var error in errorResponse.Errors.Username)
                        {
                            errorMessage += error + "\n" ?? "";
                        }
                    if (errorResponse.Errors.Password != null)
                        foreach (var error in errorResponse.Errors.Password)
                        {
                            errorMessage += error + "\n" ?? "";
                        }
                }
                else
                {
                    errorMessage = errorResponse.Title;
                }
            }
            else
            {
                errorMessage = httpResult;
            }

            return errorMessage;
        }
    }
}