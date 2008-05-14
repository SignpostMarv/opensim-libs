using System;
using System.Net;
using HttpServer.Helpers;
using HttpServer.HttpModules;
using HttpServer.Language;
using HttpServer.Language.Memory;
using HttpServer.Rendering;
using HttpServer.Rendering.Haml;
using HttpServer.Rendering.Tiny;
using TestApp.Controllers;
using TestApp.Models;
using Tiny.Data;

namespace TestApp
{
    class Program
    {
        private const string ConnectionString =
            "Server=localhost; Port=5432; Userid=postgres; password=postgres; Database=testcases; Protocol=3; Pooling=true;";

        private const string DataDriver =
            "Tiny.Data.Drivers.PostgreSql.PostgreSqlConnection, Tiny.Data.Drivers.PostgreSQL";

        private static LanguageManager _lang;

        private static DataManager _dataManager;

        public static DataManager DataManager
        {
            get { return _dataManager; }
        }

        public static LanguageManager Lang
        {
            get { return _lang; }
        }

        static void Main(string[] args)
        {
            _lang = new MemLanguageManager();
            YamlWatcher.LoadFile("..\\..\\languages.yaml", Lang);
            FormValidator.Language = Lang.GetCategory("Validator");
            
            _dataManager = new DataManager(ConnectionString, DataDriver);

            User usr = _dataManager.Fetch<User>(1);
            if (usr == null)
            {
                usr = new User();
                usr.UserName = "admin";
                usr.Password = "admin";
                _dataManager.Create(usr);
            }

            // Template generators are used to render templates 
            // (convert code + html to pure html).
            TemplateManager mgr = new TemplateManager();
            mgr.Add("haml", new HamlGenerator());
            mgr.Add("tiny", new TinyGenerator());
            mgr.AddType(typeof(WebHelper));

            // The httpserver is quite dumb and will only serve http, nothing else.
            HttpServer.HttpServer server = new HttpServer.HttpServer();

            // a controller mode implements a MVC pattern
            // You'll add all controllers to the same module.
            ControllerModule mod = new ControllerModule();
            mod.Add(new UserController(Lang, mgr));
            mod.Add(new TestController(Lang, mgr));
            mod.Add(new ItemController(Lang, mgr));
            server.Add(mod);

            // file module will be handling files
            FileModule fh = new FileModule("/", Environment.CurrentDirectory + "\\public");
            fh.AddDefaultMimeTypes();
            server.Add(fh);

            // Let's start pure HTTP, we can also start a HTTPS listener.
            server.Start(IPAddress.Any, 8081);

            Console.ReadLine();
        }
    }
}
