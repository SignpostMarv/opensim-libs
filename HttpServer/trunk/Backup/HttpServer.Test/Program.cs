using System;
using System.Collections.Generic;
using System.Text;
using HttpServer.Test.Controllers;
using HttpServer.Test.FormDecoders;
using HttpServer.Test.Renderers;

namespace HttpServer.Test
{
    class Program
    {


        static void Main(string[] args)
        {
            HamlTest ytest = new HamlTest();
            ytest.TestLayout();

            DecoderProviderTest dpt = new DecoderProviderTest();
            dpt.Setup();

            RequestControllerTest test = new RequestControllerTest();
            test.Setup();
            test.TestBinaryMethod();
        }
    }
}
