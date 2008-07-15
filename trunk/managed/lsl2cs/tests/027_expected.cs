using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLString s = "";
            LSL_Types.LSLInteger x = 1;
            s = (LSL_Types.LSLString) (x++);
            s = (LSL_Types.LSLString) (x);
            s = (LSL_Types.LSLString) (new LSL_Types.Vector3(0.0, 0.0, 0.0));
            s = (LSL_Types.LSLString) (new LSL_Types.Quaternion(1.0, 1.0, 1.0, 1.0));
            s = (LSL_Types.LSLInteger) ("1");
            s = (LSL_Types.LSLString) (llSomethingThatReturnsInteger());
            s = (LSL_Types.LSLString) (134);
            s = (LSL_Types.LSLString) (x ^ y | (z && l)) + (LSL_Types.LSLString) (x + y - 13);
            llOwnerSay("s is: " + s);
        }
    }
}
