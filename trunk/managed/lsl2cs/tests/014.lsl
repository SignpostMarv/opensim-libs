// this test tests +=, -=, *=, /=, %=

string globalString;
integer globalInt = 14;

string onefunc(string addition)
{
    globalInt -= 2;

    globalString += addition;
    return "Hi " +
           "from " +
           "onefunc()! " + globalString;
}

default
{
    touch_start(integer num_detected)
    {
        llSay(2000, onefunc());

        integer x = 2;
        x *= 3;
        x /= 14 + -2;
        x %= 10;
    }
}
