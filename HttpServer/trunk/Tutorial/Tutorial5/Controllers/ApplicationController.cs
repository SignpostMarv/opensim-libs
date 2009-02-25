using System.Globalization;
using System.Threading;
using Fadd.Globalization;
using HttpServer.Controllers;
using HttpServer.Rendering;

namespace Tutorial.Tutorial5.Controllers
{
    /// <summary>
    /// Base controller for all our web controllers.
    /// </summary>
    public abstract class ApplicationController : ViewController
    {
        private static LanguageNode _language;
        private static LanguageNode _validationLanguage;

        /// <summary>
        /// Create a new <see cref="ViewController"/>.
        /// </summary>
        protected ApplicationController(TemplateManager mgr, LanguageNode language) : base(mgr)
        {
            Language = language.GetNodeUnsafe(char.ToUpper(ControllerName[0]) + ControllerName.Substring(1) ) ?? language;

            // fetch validation language.
            LanguageNode node = language;
            while (node.ParentNode != null)
                node = language.ParentNode;
            _validationLanguage = node.GetNode("Validation");
        }

        /// <summary>
        /// Create a new <see cref="ViewController"/>.
        /// </summary>
        /// <param name="controller">prototype to copy information from.</param>
        protected ApplicationController(ViewController controller) : base(controller)
        {
        }

        [BeforeFilter]
        protected bool CheckLogin()
        {
            if (UserName == null && Request.Uri.AbsolutePath != "/user/login/")
            {
                Response.Redirect("/user/login/");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Logged in user
        /// </summary>
        public string UserName
        {
            get
            {
                return (string)Session["UserName"];
            }
            set
            {
                Session["UserName"] = value;
            }
        }

        public static LanguageNode Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public static LanguageNode ValidationLanguage
        {
            get
            {
                return _validationLanguage;
            }
        }
        protected override string RenderAction(string method, params object[] args)
        {
            Arguments.Add("Language", Language);
            return base.RenderAction(method, args);
        }

        protected override void SetupRequest(HttpServer.IHttpRequest request, HttpServer.IHttpResponse response, HttpServer.Sessions.IHttpSession session)
        {
            int lcid = 1033;
            if (request.QueryString.Contains("lcid"))
            {
                if (int.TryParse(request.QueryString["lcid"].Value, out lcid))
                    session["lcid"] = lcid;
            }

            if (session["lcid"] != null)
                lcid = (int) session["lcid"];

            Thread.CurrentThread.CurrentCulture = new CultureInfo(lcid);

            base.SetupRequest(request, response, session);
        }
    }
}