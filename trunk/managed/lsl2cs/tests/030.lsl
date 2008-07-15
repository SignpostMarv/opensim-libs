// let's test jumps

default
{
    touch_start(integer num_detected)
    {
        jump here;
        llOwnerSay("Uh oh, the jump didn't work");
        @here;
        llOwnerSay("After the jump");
    }
}
