// testing lists

default
{
    touch_start(integer num_detected)
    {
        list l = [];
        list m = [1, two, "three", <4.0, 4.0, 4.0>, 5 + 5];
        llCallSomeFunc(1, llAnotherFunc(), [1, 2, 3]);
    }
}
