using Blazored.Toast;
using DatingApp.Blazor.Data;
using DatingApp.Blazor.Services;
using Fluxor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace DatingApp.Blazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Startup).Assembly);
            });

            services.AddTransient<AuthService>();
            services.AddTransient<UserService>();
            services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001")
            });

            services.AddBlazoredToast();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.Use(next => context =>
            //{
            //    var endpoint = context.GetEndpoint();
            //    if (endpoint is null)
            //    {
            //        return Task.CompletedTask;
            //    }

            //    Console.WriteLine($"Endpoint: {endpoint.DisplayName}");

            //if (endpoint is RouteEndpoint routeEndpoint)
            //{
            //    Console.WriteLine("Endpoint has route pattern: " +
            //        routeEndpoint.RoutePattern.RawText);
            //}

            //foreach (var metadata in endpoint.Metadata)
            //{
            //    Console.WriteLine($"Endpoint has metadata: {metadata}");
            //}

            //return Task.CompletedTask;
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}