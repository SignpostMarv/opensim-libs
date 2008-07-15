using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLFloat y = 1.1;
            y = 1.123E3;
            y = 1.123e3;
            y = 1.123E+3;
            y = 1.123e+3;
            y = 1.123E-3;
            y = 1.123e-3;
            y = .4;
            y = -1.123E3;
            y = -1.123e3;
            y = -1.123E+3;
            y = -1.123e+3;
            y = -1.123E-3;
            y = -1.123e-3;
            y = -.4;
            y = 12.3 + -1.45E3 - 1.20e-2;
        }
    }
}
