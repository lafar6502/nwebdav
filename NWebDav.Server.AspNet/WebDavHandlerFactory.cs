using System.Security.Principal;
using System.Web;

namespace NWebDav.Server.AspNet
{
    public class WebDavHandlerFactory : IHttpHandlerFactory
    {
        private readonly IWebDavDispatcher _webDavDispatcher;

        public string HandlerPath { get; set; }

        public WebDavHandlerFactory(IWebDavDispatcher webDavDispatcher)
        {
            _webDavDispatcher = webDavDispatcher;

        }

        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            var t0 = this;
            var usr = context.User;
            if (usr is WindowsPrincipal)
            {
                var wp = usr as WindowsPrincipal;
            }

            return new WebDavHandler(_webDavDispatcher)
            {
                HandlerPath = this.HandlerPath
            };
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }
    }
}