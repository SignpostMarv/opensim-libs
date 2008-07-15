using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        LSL_Types.LSLString globalString = "";
        LSL_Types.LSLInteger globalInt = 14;
        LSL_Types.LSLString onefunc(LSL_Types.LSLString addition)
        {
            globalInt -= 2;
            globalString += addition;
            return "Hi " + "from " + "onefunc()! " + globalString;
        }
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            llSay(2000, onefunc());
            LSL_Types.LSLInteger x = 2;
            x *= 3;
            x /= 14 + -2;
            x %= 10;
        }
    }
}
