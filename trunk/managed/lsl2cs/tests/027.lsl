// let's test typecasts

default
{
    touch_start(integer num_detected)
    {
        string s = "";
        integer x = 1;

        s = (string) x++;
        s = (string) x;
        s = (string) <0., 0., 0.>;
        s = (string) <1., 1., 1., 1.>;
        s = (integer) "1";
        s = (string) llSomethingThatReturnsInteger();
        s = (string) 134;
        s = (string) (x ^ y | (z && l)) + (string) (x + y - 13);
        llOwnerSay("s is: " + s);
    }
}
