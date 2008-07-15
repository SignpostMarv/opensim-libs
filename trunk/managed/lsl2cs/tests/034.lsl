default
{
    state_entry()
    {
        vector v = llGetPos();
        v.z += 4;
        v.z -= 4;
        v.z *= 4;
        v.z /= 4;
        v.z %= 4;
    }
}
