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
        /// <param name="str">原始字符串</param>
        /// <returns>生成32位MD5密文</returns>
        public static string MD5_32(string pwd)
        {
            StringBuilder str = new();
            using MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(pwd);
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();

            for (int i = 0; i < md5data.Length; i++)
            {
                str.Append(md5data[i].ToString("x").PadLeft(2, '0'));

            }
            return str.ToString();
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>生成16位MD5密文</returns>

        public static string MD5_16(string str)
        {
            using MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(str)), 4, 8);

            t2 = t2.Replace("-", "");

            return t2;
        }
        #endregion

    }
}
