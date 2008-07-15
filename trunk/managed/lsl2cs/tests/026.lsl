// let's test a few more operators

default
{
    touch_start(integer num_detected)
    {
        integer x = 2;
        integer y = 1;
        integer z = x ^ y;
        x = ~ z;
        x = ~(y && z);
        y = x >> z;
        z = y << x;
    }
}
