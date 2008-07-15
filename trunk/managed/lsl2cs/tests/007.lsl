default
{
    touch_start(integer num_detected)
    {
        llOwnerSay("Testing, 1, 2, 3");
        llSay(0, "I can hear you!");
        some_custom_function(1, 2, 3 +x, 4, "five", "arguments");
    }
}
