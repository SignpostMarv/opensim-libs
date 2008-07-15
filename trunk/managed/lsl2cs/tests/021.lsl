// let's test complex logical expressions

default
{
    touch_start(integer num_detected)
    {
        integer x = 1;
        integer y = 0;

        if(x && y) llSay(0, "Hello");
        if(x || y) 
        {
            llSay(0, "Hi");
            integer r = 3;
            return;
        }

        if (x && y || z) llSay(0, "x is true");
        else llSay(0, "x is false");

        if (x == y) llSay(0, "x is true");
        else if (y < x) llSay(0, "y is true");
        else llSay(0, "Who needs x and y anyway?");

        if (x > y) llSay(0, "x is true");
        else if (y <= x)
        {
            llSay(0, "uh-oh, y is true, exiting");
            return;
        }
        else llSay(0, "Who needs x and y anyway?");

        // and now for my last trick
        if (x >= y) llSay(0, "x is true");
        else if (y != x) llSay(0, "y is true");
        else if (!z) llSay(0, "z is true");
        else if (!(a && b)) llSay(0, "a is true");
        else if (b) llSay(0, "b is true");
        else if (v) llSay(0, "v is true");
        else llSay(0, "Everything is lies!");
    }
}
