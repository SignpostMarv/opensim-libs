using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.list l = new LSL_Types.list();
            LSL_Types.list m = new LSL_Types.list(1, two, "three", new LSL_Types.Vector3(4.0, 4.0, 4.0), 5 + 5);
            llCallSomeFunc(1, llAnotherFunc(), new LSL_Types.list(1, 2, 3));
        }
    }
}
