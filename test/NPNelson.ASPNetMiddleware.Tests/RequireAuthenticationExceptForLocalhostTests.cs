using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NPNelson.ASPNetMiddleware.Middleware;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace NPNelson.ASPNetMiddleware.Tests
{
    public class RequireAuthenticationExceptForLocalhostTests
    {
        [Theory]
        [InlineData(false,false,true)]
        [InlineData(false, true, false)]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        public async Task AuthenticationLocalHostTest(bool runningOnLocal,bool authenticate,bool shouldRequire)
        {
            //Arrange
            var context = new DefaultHttpContext();

            if (!runningOnLocal) //if we leave Remote and Local IP addresses null, it will assume it's running on localhost
            {
                context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.1");
            }

            if (authenticate)
            {
                context.User = new System.Security.Claims.ClaimsPrincipal();
                context.User.AddIdentity(new System.Security.Claims.ClaimsIdentity("unitTestAuthentication"));
                context.User.AddIdentity(new System.Security.Claims.ClaimsIdentity());
            }
           
            //just add a delegate so we can test to make sure it got called
            RequestDelegate next = x =>
            {
                x.Response.ContentType = "application/xml";  //just picked an arbitrary non default contenttype
                return Task.FromResult<object>(null);
            };

            var middleware = new RequireAuthenticationExceptForLocalHostMiddleware(next);

            //Act
            await middleware.Invoke(context);

            //Assert
            if (shouldRequire)
            {
                context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized,$"RunningOnLocal={runningOnLocal} Authenticate={authenticate}");
            }
            else
            {
                context.Response.ContentType.Should().Be("application/xml",$"RunningOnLocal={runningOnLocal} Authenticate={authenticate}"); //make sure our delegate got called
            }
        }
    }
}

