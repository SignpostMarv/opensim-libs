using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLInteger x = 2;
            LSL_Types.LSLInteger y = 1;
            LSL_Types.LSLInteger z = x ^ y;
            x = ~z;
            x = ~(y && z);
            y = x >> z;
            z = y << x;
        }
    }
}
