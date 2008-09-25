using System.Collections.Specialized;
using Fadd;
using Fadd.Globalization;
using Fadd.Validation;
using HttpServer;
using HttpServer.Controllers;
using HttpServer.Helpers;
using HttpServer.Rendering;
using Tutorial.Tutorial5.Models;

namespace Tutorial.Tutorial5.Controllers
{
    public class UserController : ApplicationController
    {

        /// <summary>
        /// This constructor is never called by the framework, only by you when creating the prototype.
        /// that's why we can set the LanguageNode as static.
        /// </summary>
        public UserController(TemplateManager mgr, LanguageNode modelLanguage) : base(mgr, modelLanguage)
        {
            DefaultMethod = "Main";
        }

        /// <summary>
        /// Create a new <see cref="UserController"/>.
        /// </summary>
        /// <param name="controller">prototype to copy information from.</param>
        public UserController(ViewController controller) : base(controller)
        {
        }

        public string Login()
        {
            if (Request.Method == Method.Post)
            {
                FormValidator validator = new FormValidator(Request.Form, Errors);
                string userName = validator.Letters("UserName", true);
                string password = validator.LettersOrDigits("Password", true);
                if (validator.ContainsErrors)
                    return RenderErrors("Login", "UserName", userName, "Password", password);

                // validate login


                UserName = userName; // save it in the session

                // and redirect 
                Response.Redirect("/user/");
                return null;
            }

            return Render("UserName", string.Empty, "Password", string.Empty);
        }

        public string Main()
        {
            return Render("message", "Welcome " + UserName);
        }

        public string Settings()
        {
            User user = new User(); // todo: here you should really load user from the DB
            user.UserName = UserName;

            if (Request.Method == Method.Post)
            {
                // load stuff from the web form
                foreach (HttpInputItem item in Request.Form["user"])
                    Property.Set(user, item.Name, item.Value == string.Empty ? null : item.Value);

                // validate input
                LocalizedValidator validator = new LocalizedValidator(ValidationLanguage);
                NameValueCollection errors = validator.Validate(null, user, Language);
                if (errors.Count != 0)
                    return RenderErrors(errors, "settings", "user", user);

                // and here you should save it to the database.


                // and do something when you are done.
                Response.Redirect("/user/");
                return null;
            }

            // just render the web form.
            return Render("user", user);
        }

        /// <summary>
        /// Make a clone of this controller
        /// </summary>
        /// <returns>a new controller with the same base information as this one.</returns>
        public override object Clone()
        {
            return new UserController(this);
        }
    }
}