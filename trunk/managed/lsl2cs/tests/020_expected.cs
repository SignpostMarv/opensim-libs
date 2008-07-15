using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLInteger x = 1;
            if (x)
                llSay(0, "Hello");
            if (1)
            {
                llSay(0, "Hi");
                LSL_Types.LSLInteger r = 3;
                return ;
            }
            if (f(x))
                llSay(0, "f(x) is true");
            else
                llSay(0, "f(x) is false");
            if (x + y)
                llSay(0, "x + y is true");
            else
            if (y - x)
                llSay(0, "y - x is true");
            else
                llSay(0, "Who needs x and y anyway?");
            if (x * y)
                llSay(0, "x * y is true");
            else
            if (y / x)
            {
                llSay(0, "uh-oh, y / x is true, exiting");
                return ;
            }
            else
                llSay(0, "Who needs x and y anyway?");
            if (x % y)
                llSay(0, "x is true");
            else
            if (y & x)
                llSay(0, "y is true");
            else
            if (z | x)
                llSay(0, "z is true");
            else
            if (a * (b + x))
                llSay(0, "a is true");
            else
            if (b)
                llSay(0, "b is true");
            else
            if (v)
                llSay(0, "v is true");
            else
                llSay(0, "Everything is lies!");
        }
    }
}
