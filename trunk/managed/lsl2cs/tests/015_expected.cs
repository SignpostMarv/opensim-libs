using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.Vector3 y = new LSL_Types.Vector3(1.2, llGetMeAFloat(), 4.4);
            LSL_Types.Quaternion x = new LSL_Types.Quaternion(0.1, 0.1, one + 2, 0.9);
            y = new LSL_Types.Vector3(0.1, 0.1, 1.1 - three - two + eight * 8);
        }
    }
}
