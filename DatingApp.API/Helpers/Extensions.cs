using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response,
                                               string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddPagination(this HttpResponse response,
                                         int currentPage,
                                         int itemsPerPage,
                                         int totalItems,
                                         int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage,
                                                        itemsPerPage,
                                                        totalItems,
                                                        totalPages);

            response.Headers.Add("Pagination", JsonSerializer.Serialize(
                paginationHeader,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

        public static int CalculateAge(this DateTime dateOfBirth)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.AddYears(age) > DateTime.Today) age--;

            return age;
        }
    }
}