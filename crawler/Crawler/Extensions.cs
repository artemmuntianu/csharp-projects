using System.Text.RegularExpressions;

namespace Crawler
{
    public static class Extensions
    {
        public static string ExtractNumber(this string str)
        {
            var result = string.Empty;
            var dbl = Regex.Match(str, @"\d+.\d+").Value;
            if (!string.IsNullOrEmpty(dbl))
            {
                result = dbl;
            }
            else
            {
                var integer = Regex.Match(str, @"\d+").Value;
                if (!string.IsNullOrEmpty(integer))
                {
                    result = integer;
                }
            }
            return result;
        }

        public static double? ParseToDoubleNullable(this string val)
        {
            return string.IsNullOrEmpty(val) ? default(double?) : double.Parse(val);
        }

        public static int? ParseToIntNullable(this string val)
        {
            return string.IsNullOrEmpty(val) ? default(int?) : int.Parse(val);
        }
    }
}
