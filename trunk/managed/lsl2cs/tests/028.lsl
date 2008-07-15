// let's test states

default
{
    touch_start(integer num_detected)
    {
        llSay(0, "Going to state 'statetwo'");
        state statetwo;
    }
}

state statetwo
{
    state_entry()
    {
        llSay(0, "Going to the default state");
        state default;
    }
}
