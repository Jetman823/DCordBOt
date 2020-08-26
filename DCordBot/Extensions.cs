namespace DCordBot
{
    public static class Extensions
    {
        public static bool Anyof(this string haystack, params string[] needles)
        {

            foreach (string needle in needles)
            {
                if (haystack.Contains(needle))
                    return true;
            }

            return false;
        }
    }
}
