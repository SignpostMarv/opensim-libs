// let's test x = y = 5 type expressions

default
{
    touch_start(integer num_detected)
    {
        integer x;
        integer y;
        x = y = 5;
        x += y -= 5;
        llOwnerSay("x is: " + (string) x + ", y is: " + (string) y);
    }
}
