using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            llOwnerSay("Testing, 1, 2, 3");
            llSay(0, "I can hear you!");
            some_custom_function(1, 2, 3 + x, 4, "five", "arguments");
        }
    }
}
