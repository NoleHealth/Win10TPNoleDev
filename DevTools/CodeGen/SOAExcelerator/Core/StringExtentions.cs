using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDesign
{
    public static class StringExtentions
    {
        public static string FirstCharToLower(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            if (value.Length == 1)
                return value.ToLower();
            return string.Concat(value.Substring(0, 1).ToLower(), value.Substring(1));
        }

        public static string FirstCharToUpper(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            if (value.Length == 1)
                return value.ToLower();
            return string.Concat(value.Substring(0, 1).ToUpper(), value.Substring(1));
        }
        public static string ToUnderscoreAndUpper(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Replace(" ", "_").ToUpper();
            
        }
        
    }
}
