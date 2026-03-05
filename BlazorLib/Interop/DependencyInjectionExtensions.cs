using Microsoft.Extensions.DependencyInjection;

namespace BlazorLib.Interop
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Registers BlazorLib interop services.
        /// </summary>
        public static IServiceCollection AddBlazorLibInterop(this IServiceCollection services)
        {
            services.AddSingleton<IInteropApi, InteropApi>();
            return services;
        }
    }
}

