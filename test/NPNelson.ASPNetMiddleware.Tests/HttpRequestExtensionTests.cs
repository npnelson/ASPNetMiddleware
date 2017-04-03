using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using Xunit;

namespace NPNelson.ASPNetMiddleware.Tests
{
    public class HttpRequestExtensionTests
    {
        [Theory]
        [InlineData(null,null,true)]
        [InlineData("192.168.1.1", null, false)] //remoteIP is not localhost
        [InlineData("127.0.0.1", null, true)] //remoteIP is localhost
        [InlineData(null, "127.0.0.1", false)] //if the remoteIP doesn't come across, we want to ignore the localIP (unless localIP is null)
        [InlineData(null, "192.168.1.1", false)]//same here, if the remoteIP doesn't come across, we want to ignore the localIP (unless localIP is null)
        [InlineData("192.168.1.1", "192.168.1.1", true)] //when remoteIP and localIP are same, it can be considered a localrequest regardless of the address
        [InlineData("192.168.1.1", "192.168.1.2", false)] //obviously not localhost
        [InlineData("192.168.1.1", "127.0.0.1", false)] //again, if remoteIP is not null and not localhost, it is not a localrequest
        [InlineData("127.0.0.1", "127.0.0.1", true)] //obviously a localrequest
        public void LocalhostTest(string remoteIPAddress,string localIPAddress, bool isLocal)
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = GetIPAddressFromString(remoteIPAddress);
            context.Connection.LocalIpAddress = GetIPAddressFromString(localIPAddress);

            var result = context.Request.IsLocal();

            result.Should().Be(isLocal,$"Remote IP={remoteIPAddress} LocalIP={localIPAddress}");
        }

        private IPAddress GetIPAddressFromString(string ipAddress)
        {
            if (ipAddress == null) return null;
            return IPAddress.Parse(ipAddress);
        }
     
    }
}
