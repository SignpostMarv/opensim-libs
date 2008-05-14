using System;
using System.Collections.Generic;
using System.IO;
using HttpServer.Controllers;
using HttpServer.Exceptions;
using HttpServer.Rendering;

namespace HttpServer.Sample.Controllers
{
    public class UserController : RequestController
    {
        private TemplateManager _templateMgr;

        public UserController(TemplateManager mgr)
        {
            _templateMgr = mgr;
            DefaultMethod = "World";
        }

        public UserController(UserController controller) : base(controller)
        {
            _templateMgr = controller._templateMgr;
        }

        public string Hello()
        {
            return Render("text", "Hello World!");
        }

        private string Render(params object[] args)
        {
            try
            {
                string pageTemplate = _templateMgr.Render("views\\user\\" + MethodName + ".haml", args);
                return _templateMgr.Render("views\\layouts\\application.haml", "text", pageTemplate);
            }
                catch(FileNotFoundException err)
                {
                    throw new NotFoundException("Failed to find template. Details: " + err.Message, err);
                }
                catch(InvalidOperationException err)
                {
                    throw new InternalServerException("Failed to render template. Details: " + err.Message, err);
                }
                catch (CompileException err)
                {
                    throw new InternalServerException("Failed to compile template. Details: " + err.Message, err);
                }
            catch (ArgumentException err)
            {
                throw new InternalServerException("Failed to render templates", err);
            }
        }

        public override object Clone()
        {
            return new UserController(this);
        }

        public string World()
        {
            return "Mothafucka";
        }
    }
}
