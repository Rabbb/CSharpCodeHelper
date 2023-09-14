using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLib01
{
    public static class DecodeHelper
    {

        /// <summary>
        /// Base64解密
        /// </summary>
        public static string DecodeBase64(Encoding encode, string result) => encode.GetString(Convert.FromBase64String(result));

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        public static string DecodeBase64(string result) => DecodeBase64(Encoding.UTF8, result);

        public static byte[] FromHexString(string hex_string)
        {
            byte[] bytes = new byte[hex_string.Length / 2];

            for (int i = 2; i < hex_string.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex_string.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}
