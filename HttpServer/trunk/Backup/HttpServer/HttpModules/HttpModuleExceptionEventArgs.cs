using System;

namespace HttpServer.HttpModules
{
    /// <summary>
    /// Used to inform http server that 
    /// </summary>
    public class HttpModuleExceptionEventArgs : EventArgs
    {
        private readonly Exception _exception;

        public HttpModuleExceptionEventArgs(Exception e)
        {
            _exception = e;
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }
}
