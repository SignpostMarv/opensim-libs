// let's test hex integers

default
{
    touch_start(integer num_detected)
    {
        integer x = 0x23;
        integer x = 0x2f34B;
        integer x = 0x2F34b;
        integer x = 0x2F34B;
        integer x = 0x2f34b;
    }
}
