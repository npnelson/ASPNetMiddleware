using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

public sealed class RequireHttpsExceptForLocalhostMiddleware
{
    private readonly RequestDelegate _next;
    public RequireHttpsExceptForLocalhostMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.IsHttps || context.Request.IsLocal())
        {
            await _next(context);
        }
        else
        {
            var request = context.Request;
            var newUrl = string.Concat(
               "https://",
               request.Host.ToUriComponent(),
               request.PathBase.ToUriComponent(),
               request.Path.ToUriComponent(),
               request.QueryString.ToUriComponent());

            context.Response.Redirect(newUrl);
        }        
    }
}

namespace Microsoft.AspNetCore.Builder
{
    public static class RequireHttpsExceptForLocalHostMiddlewareExtensions
    {
        /// <summary>
        /// This is useful in webapi scenarioes where you want to require https except for when testing on localhost
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequireHttpsExceptForLocalHostMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequireHttpsExceptForLocalhostMiddleware>();
        }
    }
}

