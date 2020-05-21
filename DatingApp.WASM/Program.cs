using Blazored.Toast;
using DatingApp.WASM.Services;
using Fluxor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DatingApp.WASM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Program).Assembly);
            });

            builder.Services.AddTransient(sp => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:4001/api")
            });

            builder.Services.AddTransient<AuthService>();
            builder.Services.AddTransient<UserService>();
            builder.Services.AddBlazoredToast();

            await builder.Build().RunAsync();
        }
    }
}