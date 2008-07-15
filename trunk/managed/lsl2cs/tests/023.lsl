// let's test do-while loops

default
{
    touch_start(integer num_detected)
    {
        integer x = 1;
        integer y = 0;

        do llSay(0, "And we're doing...");
        while (x);

        do
        {
            llSay(0, "I like it here. I wish we could stay here forever.");
            y--;
        } while (y);
    }
}
