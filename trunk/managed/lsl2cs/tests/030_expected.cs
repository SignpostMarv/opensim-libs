using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            goto here;
            llOwnerSay("Uh oh, the jump didn't work");
            here:
            llOwnerSay("After the jump");
        }
    }
}
