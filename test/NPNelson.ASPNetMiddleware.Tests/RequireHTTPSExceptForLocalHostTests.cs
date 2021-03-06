﻿using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace NPNelson.ASPNetMiddleware.Tests
{
    public class RequireHTTPSExceptForLocalhostTests
    {
        [Theory]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        public async Task RequireHTTPSMiddlewareTest(bool runningOnLocal, bool isHTTPS, bool shouldRedirect)
        {
            var context = new DefaultHttpContext();

            if (!runningOnLocal) //if we leave Remote and Local IP addresses null, it will assume it's running on localhost
            {
                context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.1");
            }

            if (isHTTPS)
            {
                context.Request.Scheme = "https";
            }

            //just set up any kind of delegate to make sure our middleware executed it

            RequestDelegate next = x =>
            {
                x.Response.ContentType = "application/xml";  //just picked an arbitrary non default contenttype
                return Task.FromResult<object>(null);
            };

            var middleware = new RequireHttpsExceptForLocalhostMiddleware(next);

            //Act

            await middleware.Invoke(context);

            //Assert

            if (shouldRedirect)
            {
                context.Response.StatusCode.Should().Be((int)HttpStatusCode.Redirect, $"runningOnLocal={runningOnLocal} isHTTPS={isHTTPS}");
                context.Response.Headers["Location"][0].Should().StartWith("https", $"runningOnLocal={runningOnLocal} isHTTPS={isHTTPS}");
            }
            else
            {
                context.Response.ContentType.Should().Be("application/xml", $"runningOnLocal={runningOnLocal} isHTTPS={isHTTPS}"); //make sure our delegate got called
            }
        }

    }
}
