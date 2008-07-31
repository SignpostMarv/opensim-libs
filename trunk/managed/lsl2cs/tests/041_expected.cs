using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_state_entry()
        {
            LSL_Types.list l = new LSL_Types.list("hello");
            l = (l = new LSL_Types.list()) + l + "world";
        }
    }
}
