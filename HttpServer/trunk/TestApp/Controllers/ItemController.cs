using System;
using System.Collections.Generic;
using HttpServer.Exceptions;
using HttpServer.Helpers;
using HttpServer.Language;
using HttpServer.Rendering;
using TestApp.Models;

namespace TestApp.Controllers
{
    internal class ItemController : BaseController
    {
        public ItemController(BaseController controller)
            : base(controller)
        {
        }

        public ItemController(LanguageManager lang, TemplateManager mgr)
            : base(lang, mgr)
        {
        }

        public string Add()
        {
            return Render("testId", Id, "IsAjax", true);
        }

        public override object Clone()
        {
            return new ItemController(this);
        }

        public string Create()
        {
            TestCase testCase = new TestCase();
            FormValidator validator = new FormValidator(Request.Param["item"], Errors);
            testCase.TestId = validator.Integer("testId", true);
            testCase.Name = validator.String("name", true);
            testCase.Category = validator.String("category", true);
            testCase.Description = validator.String("description", true);
            testCase.Created = DateTime.Now;

            // Dont do anything if validation failed.
            if (validator.Errors.Count > 0)
                return RenderErrors(validator.Errors, "add", "Id", Id, "IsAjax", Request.IsAjax);

            Program.DataManager.Create(testCase);

            if (Request.IsAjax)
            {
                Response.ContentType = "text/javascript";
                return "Control.Modal.close();new Ajax.Updater('tbl', '/item/list/" + testCase.TestId + "');";
            }
            else
            {
                Response.Redirect("/test/list/");
                return null;
            }
        }

        public string List()
        {
            IList<TestCase> cases =
                Program.DataManager.FetchCollection<TestCase>(new string[] {"TestId = ?", Id}, "Category, Name", false);

            // Try to surf to http://localhost:8081/test/list/1.xml instead of just
            // http://localhost:8081/test/list/1
            if (RequestedExtension == "xml")
            {
                Response.ContentType = "text/xml";
                return XmlHelper.Serialize(cases);
            }

            // I should REALLY create partials in the Haml Generator
            if (Request.IsAjax)
                return RenderAction("_list", "items", cases);

            BottomMenu.Add(WebHelper.DialogLink("/test/add/", Language["Add"]));
            // using render action since other methods call this one directly
            return RenderAction("list", "items", cases);
        }

        public string Status()
        {
            if (string.IsNullOrEmpty(Id))
                throw new BadRequestException(Language["IdMissing"]);
            if (Request.UriParts.Length < 4)
                throw new BadRequestException(Language["MissingState"]);


            TestState state;
            string stateStr = Request.UriParts[3];

            try
            {
                if (char.IsDigit(stateStr[0]))
                    state = (TestState)Enum.ToObject(typeof(TestState), int.Parse(stateStr));
                else
                    state = (TestState)Enum.Parse(typeof(TestState), stateStr, true);
            }
            catch (FormatException err)
            {
                throw new BadRequestException("State was not valid.", err);
            }
            catch (ArgumentException err)
            {
                throw new BadRequestException("State was not valid.", err);
            }

            TestCase test = Program.DataManager.Fetch<TestCase>(Id);
            if (test == null)
                throw new NotFoundException("Test was not found.");
            test.State = state;
            Program.DataManager.Update(test);

            return RenderAction("_list", "items", GetTests(test.TestId.ToString()));
        }

        protected IList<TestCase> GetTests(string testId)
        {
            return Program.DataManager.FetchCollection<TestCase>(new string[] { "TestId = ?", testId }, "Category, Name", false);
        }

    }
}