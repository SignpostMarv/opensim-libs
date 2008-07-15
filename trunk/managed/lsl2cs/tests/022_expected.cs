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
            while (x)
                llSay(0, "To infinity, and beyond!");
            while (0 || (x && 0))
            {
                llSay(0, "Never say never.");
                return ;
            }
        }
    }
}
