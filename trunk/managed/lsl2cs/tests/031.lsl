// let's test implicitly initializing variables

default
{
    touch_start(integer num_detected)
    {
        integer i; integer j = 14;
        float f; float g = 14.0;
        string s; string t = "Hi there";
        list l; list m = [1, 2, 3];
        vector v; vector w = <1.0, 0.1, 0.5>;
        rotation r; rotation u = <0.8, 0.7, 0.6, llSomeFunc()>;
        key k; key n = "ping";
    }
}
