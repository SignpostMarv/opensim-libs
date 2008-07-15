using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        LSL_Types.LSLString onefunc()
        {
            return "Hi from onefunc()!";
        }
        void twofunc(LSL_Types.LSLString s)
        {
            llSay(1000, s);
        }
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            llSay(2000, onefunc());
            twofunc();
        }
    }
}
