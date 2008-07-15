// this test tests custom defined functions

string onefunc()
{
    return "Hi from onefunc()!";
}

twofunc(string s)
{
    llSay(1000, s);
}

default
{
    touch_start(integer num_detected)
    {
        llSay(2000, onefunc());
        twofunc();
    }
}
