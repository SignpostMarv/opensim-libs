using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLInteger i = 0;
            LSL_Types.LSLInteger j = 14;
            LSL_Types.LSLFloat f = 0.0;
            LSL_Types.LSLFloat g = 14.0;
            LSL_Types.LSLString s = "";
            LSL_Types.LSLString t = "Hi there";
            LSL_Types.list l = new LSL_Types.list();
            LSL_Types.list m = new LSL_Types.list(1, 2, 3);
            LSL_Types.Vector3 v = new LSL_Types.Vector3(0.0, 0.0, 0.0);
            LSL_Types.Vector3 w = new LSL_Types.Vector3(1.0, 0.1, 0.5);
            LSL_Types.Quaternion r = new LSL_Types.Quaternion(0.0, 0.0, 0.0, 0.0);
            LSL_Types.Quaternion u = new LSL_Types.Quaternion(0.8, 0.7, 0.6, llSomeFunc());
            LSL_Types.LSLString k = "";
            LSL_Types.LSLString n = "ping";
        }
    }
}
