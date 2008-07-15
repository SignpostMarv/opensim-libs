using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLInteger y = 0;
            LSL_Types.LSLInteger x = 14 + 6;
            y = 12 - 3;
            y = 12 * 3;
            y = 12 / 3;
            y = 12 | 3;
            y = 12 & 3;
            y = 12 % 3;
            y = 12 + 45 - 20 * x / 23 | 1 & x + y;
        }
    }
}
