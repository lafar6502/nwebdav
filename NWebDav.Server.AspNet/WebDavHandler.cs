using System;
using System.Threading.Tasks;
using System.Web;

namespace NWebDav.Server.AspNet
{
    public class WebDavHandler : IHttpAsyncHandler
    {
        private readonly IWebDavDispatcher _webDavDispatcher;

        public string HandlerPath { get; set; }

        public WebDavHandler(IWebDavDispatcher webDavDispatcher)
        {
            _webDavDispatcher = webDavDispatcher;
        }

        public void ProcessRequest(HttpContext context)
        {
            ExecuteAsync(context).Wait();
        }

        public bool IsReusable => true;

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            // Dispatch the request. Note we don't use the ConfigureAwait(false), 
            // because we want to retain the HTTP context inside the controller.
            var task = ExecuteAsync(context);

            var tcs = new TaskCompletionSource<bool>(extraData);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(true);

                cb?.Invoke(tcs.Task);
            }, TaskScheduler.Default);
            return tcs.Task;

        }

        public void EndProcessRequest(IAsyncResult result)
        {
            var task = (Task) result;
            if (task.IsFaulted)
                throw task.Exception;
            if (task.IsCanceled)
                throw new OperationCanceledException();
        }

        private Task ExecuteAsync(HttpContext context)
        {
            // Create the NWebDAV compatible context based on the ASP.NET context
            
            var up = context.Request.Path;
            if (!string.IsNullOrEmpty(context.Request.ApplicationPath) && up.StartsWith(context.Request.ApplicationPath, StringComparison.InvariantCultureIgnoreCase))
            {
                up = up.Substring(context.Request.ApplicationPath.Length);
            }
            if (!string.IsNullOrEmpty(this.HandlerPath))
            {
                if (up.StartsWith(this.HandlerPath, StringComparison.InvariantCulture))
                {
                    up = up.Substring(this.HandlerPath.Length);
                }
            }

            var aspNetContext = new AspNetContext(context, up);


            // Dispatch the request. Note we don't use the ConfigureAwait(false), 
            // because we want to retain the HTTP context inside the controller.
            return _webDavDispatcher.DispatchRequestAsync(aspNetContext);
        }
    }
}