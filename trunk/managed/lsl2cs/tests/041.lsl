default
{
    state_entry()
    {
        list l = ["hello"];
        l = (l=[]) + l + "world";
    }
}
