﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;

using System.Threading.Tasks;

namespace NPNelson.ASPNetMiddleware.Middleware
{
    public sealed class RequireAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public RequireAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var anyAuthenticated = false;
            var identities = context?.User?.Identities;
            if (identities != null) anyAuthenticated = identities.Any(x => x.IsAuthenticated);

            if (!anyAuthenticated)
            {
                //we're not running on localhost and we don't have any authentication
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await _next(context);
            }
        }
    }

    namespace Microsoft.AspNetCore.Builder
    {
        public static class RequireAuthenticationMiddlewareExtensions
        {
            /// <summary>
            /// This is useful in webapi scenarioes where you want to require authentication except for when testing on localhost
            /// </summary>
            /// <param name="builder"></param>
            /// <returns></returns>
            public static IApplicationBuilder UseRequireAuthentication(this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<RequireAuthenticationExceptForLocalHostMiddleware>();
            }
        }
    }
}
