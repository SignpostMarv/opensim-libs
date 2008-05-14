using System;
using System.Collections.Generic;
using HttpServer.Controllers;
using HttpServer;

namespace HttpServer.HttpModules
{
    /// <summary>
    /// A controller module is a part of the ModelViewController design pattern.
    /// It gives you a way to create user friendly urls.
    /// </summary>
    /// <remarks>
    /// The controller module uses the flyweight pattern which means that
    /// the memory usage will continue to increase until the module have
    /// enough objects in memory to serve all concurrent requests. The objects
    /// are reused and will not be freed.
    /// </remarks>
    /// <example>
    /// ControllerModule module = new ControllerModule();
    /// module.Add(new UserController());
    /// module.Add(new SearchController());
    /// myWebsite.Add(module);
    /// </example>
    public class ControllerModule : HttpModule
    {
        #region class ControllerContext
        private class ControllerContext
        {
            public readonly Queue<RequestController> _queue = new Queue<RequestController>();
            private readonly RequestController _prototype;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="prototype">A controller used to handle certain urls. Will be cloned for each parallell request.</param>
            public ControllerContext(RequestController prototype)
            {
                if (prototype == null)
                    throw new ArgumentNullException("prototype");

                _prototype = prototype;
            }

            public RequestController Prototype
            {
                get { return _prototype; }
            }

            public RequestController Pop()
            {
                lock (_queue)
                {
                    if (_queue.Count == 0)
                        return (RequestController)_prototype.Clone();
                    else
                        return _queue.Dequeue();
                }
            }

            public void Push(RequestController controller)
            {
                lock (_queue)
                {
                    _queue.Enqueue(controller);
                }
            }
        }
        #endregion
        private readonly Dictionary<string, ControllerContext> _controllers = new Dictionary<string, ControllerContext>();
        private WriteLogHandler _logger;

        /// <summary>
        /// The controllermodule uses the prototype design pattern
        /// to be able to create new controller objects for requests
        /// if the stack is empty.
        /// </summary>
        /// <param name="prototype">A prototype which will be cloned for each request</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidProgramException">If a controller with that name have been added already.</exception>
        public void Add(RequestController prototype)
        {
            if (prototype == null)
                throw new ArgumentNullException("prototype");

            lock (_controllers)
            {
                if (_controllers.ContainsKey(prototype.ControllerName))
                    throw new InvalidOperationException("Controller with name '" + prototype.ControllerName + "' already exists.");

                _controllers.Add(prototype.ControllerName, new ControllerContext(prototype));
            }
        }

        /// <summary>
        /// Get a prototype
        /// </summary>
        /// <param name="controllerName">in lowercase, without "Controller"</param>
        /// <returns>The controller if found; otherwise null.</returns>
        /// <example>
        /// RequestController userController = controllerModule["user"]; //fetches the class UserController
        /// </example>
        public RequestController this[string controllerName]
        {
            get
            {
                if (string.IsNullOrEmpty(controllerName))
                    return null;

                lock (_controllers)
                {
                    if (_controllers.ContainsKey(controllerName))
                        return _controllers[controllerName].Prototype;
                    else
                        return null;
                }
                
            }
        }

        /// <summary>
        /// Method that process the url
        /// </summary>
        /// <param name="request">Information sent by the browser about the request</param>
        /// <param name="response">Information that is being sent back to the client.</param>
        /// <param name="session">Session used to </param>
        public override bool Process(HttpRequest request, HttpResponse response, HttpSession session)
        {
            if (request.UriParts.Length == 0)
                return false;

            ControllerContext context;
            lock (_controllers)
            {
                if (_controllers.ContainsKey(request.UriParts[0]))
                    context = _controllers[request.UriParts[0]];
                else
                    return false;
            }

            if (!context.Prototype.CanHandle(request))
                return false;

            RequestController controller = null;
            try
            {
                lock (context)
                    controller = context.Pop();

                controller.Process(request, response, session);
            }
            finally
            {
                if (controller != null)
                {
                    lock (context)
                        context.Push(controller);
                }
                    
            }

            return true;
        }
    }
}
