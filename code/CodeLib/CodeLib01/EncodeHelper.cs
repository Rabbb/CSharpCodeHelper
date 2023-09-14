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
        public static string EncodeBase64(Encoding encode, string source) => Convert.ToBase64String(encode.GetBytes(source));

        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        public static string EncodeBase64(string source) => EncodeBase64(Encoding.UTF8, source);

        public static string ToHexString(byte[] hash, bool upper = false)
        {
            string format = upper ? "X2" : "x2";
            return string.Join("", hash.Select(bt => bt.ToString(format)));
        }
    }
}
