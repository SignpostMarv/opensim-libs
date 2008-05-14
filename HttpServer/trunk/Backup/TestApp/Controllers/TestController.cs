using System;
using System.Collections.Generic;
using HttpServer.Exceptions;
using HttpServer.Helpers;
using HttpServer.Language;
using HttpServer.Rendering;
using TestApp.Models;

namespace TestApp.Controllers
{
    internal class TestController : BaseController
    {
        public TestController(BaseController controller) : base(controller)
        {
        }

        public TestController(LanguageManager lang, TemplateManager mgr) : base(lang, mgr)
        {
        }

        public string Add()
        {
            return Render("IsAjax", Request.IsAjax);
        }

        public override object Clone()
        {
            return new TestController(this);
        }

        public string Copy()
        {
            FormValidator validator = new FormValidator(Errors);
            int id = validator.Integer("Id", Id, true);
            if (validator.Errors.Count > 0)
                return RenderErrors(MethodName);

            if (Request.Method == "POST")
                return CopyTest();

            BottomMenu.Add(WebHelper.DialogLink("/test/add/", Language["Add"]));
            BottomMenu.Add(WebHelper.Link("/test/list/", Language["List"]));
            IList<TestCase> cases = GetTests(id);
            return RenderAction("copy", "items", cases, "Id", Id);
            
        }




        private string CopyTest()
        {
            FormValidator validator = new FormValidator(Request.Form, Errors, Language);
            Test test = new Test();
            test.Name = validator.String("name", true);
            int id = validator.Integer("Id", Id, true);
            test.Created = DateTime.Now;

            if (validator.Errors.Count > 0)
                return RenderErrors(MethodName, "items", GetTests(id), "Id", Id);

            Program.DataManager.Create(test);

            // copy all test cases.
            foreach (string value in Request.Form["case"].Values)
            {
                TestCase testCase = Program.DataManager.Fetch<TestCase>(value);
                testCase.Id = 0;
                testCase.Created = DateTime.Now;
                testCase.TestId = test.Id;
                Program.DataManager.Create(testCase);
            }

            Response.Redirect("/test/show/" + test.Id);
            return null;
        }

        public string Reset()
        {
            int id = GetId();
            if (id == -1)
                return null;

            // I should really make an Execute method in the orm layer
            foreach (TestCase item in GetTests(id))
            {
                item.State = TestState.NotTested;
                Program.DataManager.Update(item);
            }

            if (Request.IsAjax)
                return RenderTemplate("item", "_list", "items", GetTests(id));

            Response.Redirect("/item/list/" + Id);
            return null;
        }

        private int GetId()
        {
            int id;
            if (!int.TryParse(Id, out id))
            {
                Errors.Add("Id", "Id cannot be empty.");
                RenderErrors(null);
                return -1;
            }

            return id;
        }

        public string List()
        {
            BottomMenu.Add(WebHelper.DialogLink("/test/add/", Language["Add"]));

            IList<Test> tests = Program.DataManager.FetchCollection<Test>();
            return Render("tests", tests);
        }

        public string Show()
        {
            int id = GetId();
            if (id == -1)
                return null;

            Test test = Program.DataManager.Fetch<Test>(id);
            if (test == null)
                throw new NotFoundException("Id was not found.");

            // render items
            string items = RenderTemplate("item", "_list", "items", GetTests(id));

            BottomMenu.Add(WebHelper.DialogLink("/item/add/" + Id, Language["AddCase"]));
            BottomMenu.Add(WebHelper.Link("/test/copy/" + Id, Language["CopyTest"]));
            BottomMenu.Add(WebHelper.AjaxUpdater("/test/reset/" + Id, Language["Reset"], "tbl"));
            return Render("test", test, "items", items);
        }

        protected IList<TestCase> GetTests(int testId)
        {
            return Program.DataManager.FetchCollection<TestCase>(new string[] { "TestId = ?", testId.ToString() }, "Category, Name", false);
        }
    }
}