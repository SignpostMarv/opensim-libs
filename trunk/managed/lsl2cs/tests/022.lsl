// let's test while loops

default
{
    touch_start(integer num_detected)
    {
        integer x = 1;
        integer y = 0;

        while (x) llSay(0, "To infinity, and beyond!");
        while (0 || (x && 0))
        {
            llSay(0, "Never say never.");
            return;
        }
    }
}
