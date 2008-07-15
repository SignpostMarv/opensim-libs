using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            llSay(0, "Going to state 'statetwo'");
            state("statetwo");
        }
        public void statetwo_event_state_entry()
        {
            llSay(0, "Going to the default state");
            state("default");
        }
    }
}
