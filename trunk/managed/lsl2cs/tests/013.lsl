// this test tests custom defined functions and global variables

string globalString;
integer globalInt = 14;
integer anotherGlobal = 20 * globalInt;

string onefunc()
{
    globalString = " ...and the global!";
    return "Hi " +
           "from " +
           "onefunc()!" + globalString;
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
