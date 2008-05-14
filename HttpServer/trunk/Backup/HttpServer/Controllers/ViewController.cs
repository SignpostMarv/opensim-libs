using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using HttpServer;
using HttpServer.Controllers;
using HttpServer.Exceptions;
using HttpServer.Helpers;
using HttpServer.Rendering;

namespace HttpServer.Controllers
{
    public abstract class ViewController : RequestController
    {
        private readonly TemplateManager _templateMgr;
        private NameValueCollection _errors = new NameValueCollection();
        private bool _includeLayoutInAjaxRequests = false;
        private string _layout = "Application";
        private string _title;
        private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();

        public ViewController(TemplateManager mgr)
        {
            _templateMgr = mgr;
        }

        public ViewController(ViewController controller)
            : base(controller)
        {
            _templateMgr = controller._templateMgr;
        }

        /// <summary>
        /// A set of errors that occured during request processing.
        /// </summary>
        /// <remarks>Errors can be rendered into templates using the WebHelper.RenderError method.</remarks>
        /// <seealso cref="WebHelper"/>
        [XmlElement("Errors")]
        protected NameValueCollection Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }

        /// <summary>
        /// True if we always should render contents inside page layouts.
        /// </summary>
        public bool IncludeLayoutInAjaxRequests
        {
            get { return _includeLayoutInAjaxRequests; }
            set { _includeLayoutInAjaxRequests = value; }
        }

        /// <summary>
        /// Which page layout to use (without file extension)
        /// </summary>
        /// <remarks>Page layouts should be places in the Views\Layouts folder.</remarks>
        public string Layout
        {
            get { return _layout; }
            set { _layout = value; }
        }

        /// <summary>
        /// Page title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        protected Dictionary<string, object> Arguments
        {
            get { return _arguments; }
        }

        protected string Render(params object[] args)
        {
            return RenderAction(MethodName, args);
        }

        protected string RenderAction(string action, params object[] args)
        {
            return RenderActionWithErrors(action, _errors, args);
        }

        protected virtual string RenderActionWithErrors(string action, NameValueCollection errors, params object[] args)
        {
            // no page template specified means that everything should be rendered into the main template directly
            string pageTemplate;
            if (!string.IsNullOrEmpty(action))
            {
                // Small hack to be able to display error messages.
                object[] args2 = new object[args.Length + (_arguments.Count * 2) + 2];
                args.CopyTo(args2, 0);
                int index = args.Length - 1;
                foreach (KeyValuePair<string, object> pair in _arguments)
                {
                    args2[++index] = pair.Key;
                    args2[++index] = pair.Value;
                }
                args2[++index] = "errors";
                args2[++index] = errors;

                pageTemplate = RenderTemplate(ControllerName, action, args2);
            }
            else
                pageTemplate = WebHelper.RenderErrors(_errors);

            _arguments.Clear();
            _errors.Clear();

            // 1. dont render main layout for ajax requests, since they just update partial 
            // parts of the web page.
            // 2. Dont render html layout for other stuff than html
            if (Request.IsAjax && !_includeLayoutInAjaxRequests)
                return pageTemplate;


            return RenderLayout(_layout, pageTemplate);
        }

        protected string RenderErrors(string method, params object[] arguments)
        {
            return RenderErrors(_errors, method, arguments);
        }

        protected string RenderErrors(NameValueCollection errors, string method, params object[] arguments)
        {
            if (errors.Count > 0)
            {
                if (Request.IsAjax)
                    return RenderJsErrors(errors);

                return RenderActionWithErrors(method, errors, arguments);
            }

            return null;
        }

        protected string RenderJavascript(string js)
        {
            Response.ContentType = "text/javascript";
            return js;
        }

        protected string RenderJsErrors(NameValueCollection errors)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("alert('");
            for (int i = 0; i < errors.Count; ++i)
                sb.Append(errors[i].Replace("'", "\\'") + "\\n");
            sb.Append("');");

            Response.ContentType = "text/javascript";
            return sb.ToString();
        }

        protected virtual string RenderLayout(string layoutName, string contents)
        {
            // template engine will find all layouts and take the first one with that one.
            return RenderTemplate("layouts", layoutName, "text", contents,
                                  "title", Title ?? "The page with no name");
        }

        protected string RenderTemplate(string controller, string action, params object[] args)
        {
            try
            {
                return _templateMgr.Render("views\\" + controller + "\\" + action + ".*", args);
            }
            catch (FileNotFoundException err)
            {
                throw new NotFoundException("Failed to find template. Details: " + err.Message, err);
            }
            catch (InvalidOperationException err)
            {
                throw new InternalServerException("Failed to render template. Details: " + err.Message, err);
            }
            catch (CompileException err)
            {
#if DEBUG
                throw new InternalServerException(err.Message + "<br />" + err.Data["code"] ?? "No compiled code");
#else
                throw new InternalServerException("Failed to compile template. Details: " + err.Message, err);
#endif
            }
            catch (CodeGeneratorException err)
            {
#if DEBUG
                throw new InternalServerException(err.ToString().Replace("\r\n", "<br />\r\n"), err);
#else
                throw new InternalServerException("Failed to compile template.");
#endif
            }
            catch (ArgumentException err)
            {
#if DEBUG
                throw new InternalServerException("Failed to render template, reason: " + err.ToString().Replace("\r\n", "<br />\r\n"), err);
#else
                throw new InternalServerException("Failed to render templates", err);
#endif
            }
        }

        protected override void SetupRequest(HttpRequest request, HttpResponse response, HttpSession session)
        {
            _arguments.Clear();
            _errors.Clear();
            base.SetupRequest(request, response, session);
            _layout = Request.Param["layout"].Value ?? Layout;
        }
    }
}