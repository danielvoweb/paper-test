namespace Demo
{
    public static class Helpers
    {
        public static string[] SentenceToStringArray(string sentence)
        {
            return sentence
                .ToLowerInvariant()
                .Replace(",", string.Empty)
                .Replace(".", string.Empty)
                .Split(' ');
        }
    }
}