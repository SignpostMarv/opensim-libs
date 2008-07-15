// this test tests strings, with escaped quotes and comments in strings
default
{
    touch_start(integer num_detected)
    {
        string s1 = "this is a string.";
        string s2 = "this is a string "+"with an escaped \" inside it.";
        s1 = s2+" and this "+"is a string with // comments.";

        string onemore = "[\^@]";

        string multiline = "Good evening Sir,
        my name is Steve.
        I come from a rough area.
        I used to be addicted to crack
        but now I am off it and trying to stay clean.
        That is why I am selling magazine subscriptions."; // http://www.imdb.com/title/tt0151804/quotes
    }
}
