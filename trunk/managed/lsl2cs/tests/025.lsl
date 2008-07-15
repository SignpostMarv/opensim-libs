// a curious feature of LSL that allows floats to be defined with a trailing dot

default
{
    touch_start(integer num_detected)
    {
        float y = 1.;
        y = 1.E3;
        y = 1.e3;
        y = 1.E+3;
        y = 1.e+3;
        y = 1.E-3;
        y = 1.e-3;
        y = -1.E3;
        y = -1.e3;
        y = -1.E+3;
        y = -1.e+3;
        y = -1.E-3;
        y = -1.e-3;
        y = 12. + -1.E3 - 1.e-2;
        vector v = <0.,0.,0.>;
    }
}
