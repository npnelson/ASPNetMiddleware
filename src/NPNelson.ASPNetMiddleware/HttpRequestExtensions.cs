using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Microsoft.AspNetCore.Http
{
    //http://www.strathweb.com/2016/04/request-islocal-in-asp-net-core/

    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Determines if a request originated from localhost
        /// DANGER: Be careful when running in reverse proxy scenarioes. You must have middleware running that understands the X-Forwarded headers to set the connection information, otherwise you will get the IP address of the proxy
        /// See http://www.strathweb.com/2016/04/request-islocal-in-asp-net-core/ for more details
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static bool IsLocal(this HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                if (connection.LocalIpAddress != null)
                {
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
                }
                else
                {
                    return IPAddress.IsLoopback(connection.RemoteIpAddress);
                }
            }

            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }
            return false;
        }
    }
}
