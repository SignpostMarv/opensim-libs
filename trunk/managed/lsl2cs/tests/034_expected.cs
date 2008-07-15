using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_state_entry()
        {
            LSL_Types.Vector3 v = llGetPos();
            v.z += 4;
            v.z -= 4;
            v.z *= 4;
            v.z /= 4;
            v.z %= 4;
        }
    }
}
