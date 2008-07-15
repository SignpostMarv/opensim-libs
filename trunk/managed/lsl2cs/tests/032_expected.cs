using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLInteger x = 0;
            LSL_Types.LSLInteger y = 0;
            x = y = 5;
            x += y -= 5;
            llOwnerSay("x is: " + (LSL_Types.LSLString) (x) + ", y is: " + (LSL_Types.LSLString) (y));
        }
    }
}
