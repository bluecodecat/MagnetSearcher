using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MagnetSearcher
{
    class MagnetUtility
    {

        public static long StringSize2ByteSize(string str)
        {
            string upper = str.ToUpper();

            string pattern = @"(\d+(\.\d+)?)([GMK])B";
            Regex regex = new Regex(pattern);

            var match = regex.Match(upper);

            double number = 0;
            double.TryParse(match.Groups[1].Value, out number);
            long result = 0;
            switch (match.Groups[3].Value)
            {
                case "G":
                    result = (long)(number * 1024 * 1024 * 1024);
                    break;
                case "M":
                    result = (long)(number * 1024 * 1024);
                    break;
                case "K":
                    result = (long)(number * 1024);
                    break;
            }
            return result;
        }
    }
}
