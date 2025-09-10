using MauiApp2.Service;
using MauiApp2.Services;
using Microsoft.Extensions.Logging;
using Plugin.Maui.OCR;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MauiApp2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                 .UseOcr()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");   
                });
            // ✅ Register HttpClient
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("http://fumicerti.bhed.in/") // Adjust if needed
            });
            //builder.Services.AddScoped(sp => new HttpClient
            //{
            //    BaseAddress = new Uri("http://localhost:5004/")
            //});


            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddScoped<ImageService>();
            builder.Services.AddScoped<UserService>();
        



#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
