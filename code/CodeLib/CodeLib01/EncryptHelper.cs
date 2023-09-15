using System;
using System.Security.Cryptography;
using System.Text;

namespace CodeLib01
{
    public static class EncryptHelper
    {

        #region MD5加密
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="base_string">原始字符串</param>
        /// <returns>生成32位MD5密文</returns>
        public static string MD5_32(string base_string, bool upper = false)
        {
            byte[] md5data;
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                md5data = md5.ComputeHash(Encoding.UTF8.GetBytes(base_string));
            }
            return md5data.ToHexString(upper);
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="base_string">原始字符串</param>
        /// <returns>生成16位MD5密文</returns>

        public static string MD5_16(string base_string, bool upper = false)
        {
            byte[] md5data;
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                md5data = md5.ComputeHash(Encoding.UTF8.GetBytes(base_string));
            }
            var bytes = new byte[8];
            Array.Copy(md5data, 4, bytes, 0, 8);
            return bytes.ToHexString(upper);
        }
        #endregion

    }
}
