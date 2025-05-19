// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace FeeLink.Infrastructure.Common.Authentication;
//
// public static class GoogleAuthenticationScheme
// {
//     public static IServiceCollection AddGoogleAuthenticationScheme(this IServiceCollection services, IConfiguration config)
//     {
//         services.AddAuthentication().AddCookie().AddGoogle(options =>
//         {
//             options.ClientId = config["Google:ClientId"]!;
//             options.ClientSecret = config["Google:ClientSecret"]!;
//         });
//
//         return services;
//     }
// }