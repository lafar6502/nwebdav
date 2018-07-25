﻿using System;
using System.Threading.Tasks;
using System.Web;
using NWebDav.Server.Http;

namespace NWebDav.Server.AspNet
{
    public partial class AspNetContext : IHttpContext
    {
        private readonly AspNetRequest _request;
        private readonly AspNetSession _session;
        private readonly AspNetResponse _response;

        public AspNetContext(HttpContext httpContext, string resourcePath = null)
        {
            // Assign properties
            _request = new AspNetRequest(httpContext.Request);
            _request.ResourcePath = new Uri(resourcePath, UriKind.RelativeOrAbsolute);

            _session = new AspNetSession(httpContext);
            _response = new AspNetResponse(httpContext.Response);
        }

        public IHttpRequest Request => _request;
        public IHttpResponse Response => _response;
        public IHttpSession Session => _session;

        public Task CloseAsync()
        {
            // NOP, so can be a synchronous call
            return Task.FromResult(true);
        }
    }
}