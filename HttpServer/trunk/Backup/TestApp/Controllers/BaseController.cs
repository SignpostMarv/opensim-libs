using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using HttpServer;
using HttpServer.Controllers;
using HttpServer.Language;
using HttpServer.Rendering;
using TestApp.Models;

namespace TestApp.Controllers
{
    public abstract class BaseController : ViewController
    {
        private static readonly User Empty = new User();
        private List<string> _bottomMenu = new List<string>();
        private readonly LanguageManager _langMgr;

        public BaseController(LanguageManager langMgr, TemplateManager mgr) : base(mgr)
        {
            if (langMgr == null)
                throw new ArgumentNullException("langMgr");

            _langMgr = langMgr;
        }

        public BaseController(BaseController controller)
            : base(controller)
        {
            _langMgr = controller._langMgr;
        }

        public User AuthedUser
        {
            get { return (User) AuthenticationTag; }
            set { AuthenticationTag = value; }
        }

        public List<string> BottomMenu
        {
            get { return _bottomMenu; }
            set { _bottomMenu = value; }
        }

        public LanguageCategory Language
        {
            get
            {
                return _langMgr.GetCategory(ControllerName);
            }
        }

        [BeforeFilter(FilterPosition.First)]
        protected bool Authorized()
        {
            if (MethodName == "login" || MethodName == "dologin")
                return true;

            return Authorized(0);
        }

        protected bool Authorized(int minLevel)
        {
            return true;

            if (AuthedUser == null)
            {
                Session["returnTo"] = Request.Uri.PathAndQuery;
                Response.Redirect("/user/login/");
                return false;
            }

            return true;
        }

        protected override string RenderActionWithErrors(string action, System.Collections.Specialized.NameValueCollection errors, params object[] args)
        {
            Arguments.Add("Language", Language);
            return base.RenderActionWithErrors(action, errors, args);
        }

        protected override string RenderLayout(string layoutName, string contents)
        {
            // template engine will find all layouts and take the first one with that one.
            return RenderTemplate("layouts", layoutName, "text", contents,
                                  "user", AuthedUser ?? Empty,
                                  "bottomMenuItems", _bottomMenu,
                                  "title", Title ?? "The page with no name",
                                  "Language", _langMgr.GetCategory(LanguageCategory.Default));
        }

        protected override void SetupRequest(HttpRequest request, HttpResponse response, HttpSession session)
        {
            _bottomMenu.Clear();

            SelectLanguage(request.Headers["accept-language"]);
          
            base.SetupRequest(request, response, session);
        }

        protected virtual void SelectLanguage(string langHeader)
        {
            if (string.IsNullOrEmpty(langHeader))
                return;

            string[] langs = langHeader.Split(',');
            foreach (string langStr in langs)
            {
                string language;
                int pos = langStr.IndexOf(';');
                if (pos != -1)
                    language = langStr.Remove(pos);
                else
                    language = langStr;

                // Else we'll get a report of language being neutral and can therefore not be used.
                // feel free to come up with a better solution
                if (language == "sv")
                    language = "sv-se";

                CultureInfo culture = CultureInfo.GetCultureInfoByIetfLanguageTag(language);
                if (Language.Contains(culture.LCID))
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                    break;
                }
            }
        }
    }
}