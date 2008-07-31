using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_state_entry()
        {
            LSL_Types.LSLInteger x = 0;
            while (x = 14)
                llOwnerSay("x is: " + (LSL_Types.LSLString) (x));
            if (x = 24)
                llOwnerSay("x is: " + (LSL_Types.LSLString) (x));
            do
                llOwnerSay("x is: " + (LSL_Types.LSLString) (x));
            while (x = 44);
        }
    }
}
