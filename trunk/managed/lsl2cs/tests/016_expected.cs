using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.Vector3 y = new LSL_Types.Vector3(1.2, llGetMeAFloat(), 4.4);
            x = y.x + 1.1;
            y.x = 1.1;
        }
    }
}
