default
{
    state_entry()
    {
        integer x;

        while(x = 14) llOwnerSay("x is: " + (string) x);

        if(x = 24) llOwnerSay("x is: " + (string) x);

        do
            llOwnerSay("x is: " + (string) x);
        while(x = 44);
    }
}
