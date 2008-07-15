using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLString s1 = "this is a string.";
            LSL_Types.LSLString s2 = "this is a string " + "with an escaped \" inside it.";
            s1 = s2 + " and this " + "is a string with // comments.";
            LSL_Types.LSLString onemore = "[\^@]";
            LSL_Types.LSLString multiline = "Good evening Sir,\n        my name is Steve.\n        I come from a rough area.\n        I used to be addicted to crack\n        but now I am off it and trying to stay clean.\n        That is why I am selling magazine subscriptions.";
        }
    }
}
