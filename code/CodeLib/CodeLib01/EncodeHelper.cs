using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLib01
{
    public static class EncodeHelper
    {

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <returns></returns>
        public static string EncodeBase64(string source, Encoding encode) => Convert.ToBase64String(encode.GetBytes(source));

        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        public static string EncodeBase64(string source) => EncodeBase64(source, Encoding.UTF8);

        public static string ToHexString(this byte[] hash, bool upper = false)
        {
            string format = upper ? "X2" : "x2";

            StringBuilder str = new();
            for (int i = 0; i < hash.Length; i++)
            {
                str.Append(hash[i].ToString(format));

            }
            return str.ToString();
        }
    }
}
