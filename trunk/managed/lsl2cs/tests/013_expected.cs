using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        LSL_Types.LSLString globalString = "";
        LSL_Types.LSLInteger globalInt = 14;
        LSL_Types.LSLInteger anotherGlobal = 20 * globalInt;
        LSL_Types.LSLString onefunc()
        {
            globalString = " ...and the global!";
            return "Hi " + "from " + "onefunc()!" + globalString;
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
