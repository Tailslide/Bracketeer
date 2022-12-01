using System.Text;

namespace Bracketeer.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Remove specified characters from string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string RemoveChars(this string input, params char[] chars)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (!chars.Contains(input[i]))
                    sb.Append(input[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Remove indents tabs and linebreaks and extra whitespace
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string deindent(this string s)
        {
            string s2 = s.Trim().RemoveChars('\r', '\n', '\t');
            while (s2.Contains("  ")) s2 = s2.Replace("  ", " "); // remove double spaces
            return s2;
        }



    }
}
