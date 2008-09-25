using System;
using System.IO;
using System.Net;
using HttpServer;
using HttpListener=HttpServer.HttpListener;

namespace Tutorial.Tutorial1
{
    public class Tutorial1 : Tutorial
    {
        private HttpListener listener;

        #region Tutorial Members

        public void StartTutorial()
        {
            Console.WriteLine("Welcome to Tutorial #1!");
            Console.WriteLine();
            Console.WriteLine("HttpListener allows you to handle everything yourself.");
            Console.WriteLine();
            Console.WriteLine("Browse to http://localhost:8081/hello and http://localhost:8081/goodbye to view the contents");

            listener = new HttpListener(IPAddress.Any, 8081);
            listener.RequestHandler += OnRequest;
            listener.Start(5);
        }

        private void OnRequest(IHttpClientContext context, IHttpRequest request)
        {
            // Respond is a small convenience function that let's you send one string to the browser.
            // you can also use the Send, SendHeader and SendBody methods to have total control.
            if (request.Uri.AbsolutePath == "/hello")
                context.Respond("Hello to you too!"); 

            else if (request.UriParts.Length == 1 && request.UriParts[0] == "goodbye")
            {
                HttpResponse response = new HttpResponse(context, request);
                StreamWriter writer = new StreamWriter(response.Body);
                writer.WriteLine("Goodbye to you too!");
                writer.Flush();
                response.Send();
            }
        }

        public void EndTutorial()
        {
            listener.Stop();
        }

        public string Name
        {
            get { return "Demo of HttpListener";  }
        }
        #endregion
    }
}
