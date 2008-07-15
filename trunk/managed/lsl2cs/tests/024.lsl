// let's test for loops

default
{
    touch_start(integer num_detected)
    {
        integer x = 1;
        integer y = 0;

        for(x = 10; x >= 0; x--)
        {
            llOwnerSay("Launch in T minus " + x);
            IncreaseRocketPower();
        }

        for(x = 0, y = 6; y > 0 && x != y; x++, y--) llOwnerSay("Hi " + x + ", " + y);
        for(x = 0, y = 6; ! y; x++,y--) llOwnerSay("Hi " + x + ", " + y);
    }
}
