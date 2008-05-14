using System;
using HttpServer.Language;
using HttpServer.Rendering;
using TestApp.Models;

namespace TestApp.Controllers
{
    class UserController : BaseController
    {
        public UserController(BaseController controller) : base(controller)
        {
        }

        public UserController(LanguageManager lang, TemplateManager mgr) : base(lang, mgr)
        {
        }

        public string Logout()
        {
            Session.Clear();
            return Language["Goodbye"];
        }

        public string Login()
        {
            return Render("username", "admin");
        }

        public string DoLogin()
        {
            try
            {
                User user =
                    Program.DataManager.Fetch<User>(
                        new string[]
                            {
                                "UserName = ? AND Password = ?", 
                                Request.Form["username"].Value,
                                Request.Form["password"].Value
                            });

                if (user == null)
                {
                    Errors.Add(null, Language["LoginFailed"]);
                    return RenderAction("login", "username", Request.Form["username"].Value);
                }
                else
                {
                    AuthedUser = user;
                    Response.Redirect((string)Session["returnTo"] ?? "/user/index/");
                    return null;
                }
            }
            catch (InvalidOperationException)
            {
            }

            return RenderAction("login", "username", string.Empty);
        }

        public override object Clone()
        {
            return new UserController(this);
        }
    }
}
