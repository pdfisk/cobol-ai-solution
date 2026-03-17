using Microsoft.AspNetCore.Components;

namespace BlazorLib.Interop
{
    public static class DependencyInjectionExtensions
    {
        private const string ServerUrlLocalhost = "http://localhost:5000/";
        private const string ServerUrlProduction = "https://cobol-ai-studio-d0c7533a4035.herokuapp.com/";

        /// <summary>
        /// Registers BlazorLib interop services.
        /// Server URL is http://localhost:5000 when the client is on localhost, otherwise https://cobol-ai-studio-d0c7533a4035.herokuapp.com.
        /// </summary>
        public static IServiceCollection AddBlazorLibInterop(this IServiceCollection services)
        {
            services.AddScoped<IInteropApi>(sp =>
            {
                var nav = sp.GetRequiredService<NavigationManager>();
                var baseUrl = IsLocalhost(nav.Uri) ? ServerUrlLocalhost : ServerUrlProduction;
                var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
                return new InteropApi(httpClient);
            });
            return services;
        }

        private static bool IsLocalhost(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return false;
            try
            {
                var host = new Uri(uri, UriKind.RelativeOrAbsolute).Host;
                return host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                    || host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}

