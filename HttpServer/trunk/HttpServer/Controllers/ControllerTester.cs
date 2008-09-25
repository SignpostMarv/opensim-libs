using System;
using System.Collections.Generic;
using System.IO;
using HttpServer.Sessions;
using Xunit;

namespace HttpServer.Controllers
{
    /// <summary>
    /// Used to simply testing of controls.
    /// </summary>
    public class ControllerTester
    {
        /// <summary>
        /// Fake host name, default is "http://localhost"
        /// </summary>
        public string HostName = "http://localhost";

        /// <summary>
        /// Session used if null have been specified as argument to one of the class methods.
        /// </summary>
        public IHttpSession DefaultSession = new MemorySession("abc");

        /// <summary>
        /// Send a GET request to a controller.
        /// </summary>
        /// <param name="controller">Controller receiving the post request.</param>
        /// <param name="uri">Uri visited.</param>
        /// <param name="response">Response from the controller.</param>
        /// <param name="session">Session used during the test. null = <see cref="DefaultSession"/> is used.</param>
        /// <returns>body posted by the response object</returns>
        /// <example>
        /// <code>
        /// void MyTest()
        /// {
        ///     ControllerTester tester = new ControllerTester();
        ///     
        ///     MyController controller = new MyController();
        ///     IHttpResponse response;
        ///     string text = Get(controller, "/my/hello/1?hello=world", out response, null);
        ///     Assert.Equal("world|1", text);
        /// }
        /// </code>
        /// </example>
        public string Get(RequestController controller, string uri, out IHttpResponse response, IHttpSession session)
        {
            return Invoke(controller, Method.Get, uri, out response, session);
        }

        /// <summary>
        /// Send a POST request to a controller.
        /// </summary>
        /// <param name="controller">Controller receiving the post request.</param>
        /// <param name="uri">Uri visited.</param>
        /// <param name="form">Form being processed by controller.</param>
        /// <param name="response">Response from the controller.</param>
        /// <param name="session">Session used during the test. null = <see cref="DefaultSession"/> is used.</param>
        /// <returns>body posted by the response object</returns>
        /// <example>
        /// <code>
        /// void MyTest()
        /// {
        ///     // Create a controller.
        ///     MyController controller = new MyController();
        ///
        ///     // build up a form that is used by the controller.
        ///     HttpForm form = new HttpForm();
        ///     form.Add("user[firstName]", "Jonas");
        /// 
        ///     // Invoke the request
        ///     ControllerTester tester = new ControllerTester();
        ///     IHttpResponse response;
        ///     string text = tester.Get(controller, "/user/create/", form, out response, null);
        /// 
        ///     // validate response back from controller.
        ///     Assert.Equal("User 'Jonas' has been created.", text);
        /// }
        /// </code>
        /// </example>
        public string Post(RequestController controller, string uri, HttpForm form, out IHttpResponse response, IHttpSession session)
        {
            return Invoke(controller, Method.Post, uri, form, out response, session);
        }

        private string Invoke(RequestController controller, string httpMetod, string uri, out IHttpResponse response, IHttpSession session)
        {
            return Invoke(controller, httpMetod, uri, null, out response, session);
        }

        private string Invoke(RequestController controller, string httpMetod, string uri, HttpForm form, out IHttpResponse response, IHttpSession session)
        {
            HttpRequest request = new HttpRequest();
            // http header.
            request.HttpVersion = "HTTP/1.1";
            request.UriPath = uri;
            request.Method = httpMetod;
            request.Uri = new Uri(HostName + uri);
            request.AssignForm(form);

            response = new HttpResponse(null, request);
            controller.Process(request, response, session);
            response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(response.Body);
            return reader.ReadToEnd();
        }

        [Fact]
        private void TestGet()
        {
            MyController controller = new MyController();
            IHttpResponse response;
            IHttpSession session = DefaultSession;
            string text = Get(controller, "/my/hello/1?hello=world", out response, session);
            Assert.Equal("world|1", text);
        }

        [Fact]
        private void TestPost()
        {
            MyController controller = new MyController();
            IHttpResponse response;
            IHttpSession session = DefaultSession;
            HttpForm form = new HttpForm();
            form.Add("user[firstName]", "jonas");
            string text = Post(controller, "/my/hello/", form, out response, session);
            Assert.Equal("jonas", text);
        }


        private class MyController : RequestController
        {
            public string Hello()
            {
                if (Request.Method == Method.Post)
                    return Request.Form["user"]["firstName"].Value;

                return Request.QueryString["hello"].Value + "|" + Id; 
            }

            public override object Clone()
            {
                return new MyController();
            }
        }
    }
}
