
using System.Collections.Generic;
using System.Linq;

namespace Burton.Core.Common
{
    public static class StringExtensions
    {
        public static string GetStringAfterStartingString(
            this string inputString, 
            string substring)
        {
            return inputString.LastIndexOf(substring) != 0 ? 
                inputString : 
                inputString.Substring(substring.Length);
        }

        public static List<string> GetNonEmptyTokens(this string input)
        {
            return input
                    .Split(' ')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
        }
    }
}
