using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLFloat y = 1.0;
            y = 1.0E3;
            y = 1.0e3;
            y = 1.0E+3;
            y = 1.0e+3;
            y = 1.0E-3;
            y = 1.0e-3;
            y = -1.0E3;
            y = -1.0e3;
            y = -1.0E+3;
            y = -1.0e+3;
            y = -1.0E-3;
            y = -1.0e-3;
            y = 12.0 + -1.0E3 - 1.0e-2;
            LSL_Types.Vector3 v = new LSL_Types.Vector3(0.0, 0.0, 0.0);
        }
    }
}
