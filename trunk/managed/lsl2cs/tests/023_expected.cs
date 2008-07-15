using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLInteger x = 1;
            LSL_Types.LSLInteger y = 0;
            do
                llSay(0, "And we're doing...");
            while (x);
            do
            {
                llSay(0, "I like it here. I wish we could stay here forever.");
                y--;
            }
            while (y);
        }
    }
}
