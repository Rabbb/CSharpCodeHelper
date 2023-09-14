using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CodeLib01
{
    public static class SignHelper
    {

        /// <summary>
        /// 签名(MD5 + Base64)<br/>
        /// 2023-9-14 Ciaran
        /// </summary>
        /// <param name="base_string">拼接字符串</param>
        /// <returns></returns>
        public static string MD5_Base64(string base_string)
        {
            byte[] md5data;
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                md5data = md5.ComputeHash(Encoding.UTF8.GetBytes(base_string));
            }

            return Convert.ToBase64String(md5data);
        }

        /// <summary>
        /// 签名(MD5 + Base64)<br/>
        /// 2023-9-14 Ciaran
        /// </summary>
        /// <param name="base_string">拼接字符串</param>
        /// <returns></returns>
        public static string MD5_Base64_2(string base_string)
        {
            byte[] md5data;
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                md5data = md5.ComputeHash(Encoding.UTF8.GetBytes(base_string));
            }

            md5data = Encoding.UTF8.GetBytes(md5data.ToHexString());
            return Convert.ToBase64String(md5data);
        }

        /// <summary>
        /// 签名(HmacSha256 + Base64)<br/>
        /// 2023-9-14 Ciaran
        /// </summary>
        /// <param name="base_string">拼接字符串</param>
        /// <param name="nonce_key">密匙 + 随机字符串</param>
        /// <returns></returns>
        public static string HmacSHA256_Base64(string base_string, string nonce_key)
        {
            byte[] hash;
            using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(nonce_key)))
            {
                hash = mac.ComputeHash(Encoding.UTF8.GetBytes(base_string));
            }

            return Convert.ToBase64String(hash);
        }


        /// <summary>
        /// 签名(HmacSha256 + Base64)<br/>
        /// 2023-9-14 Ciaran
        /// </summary>
        /// <param name="base_string">拼接字符串</param>
        /// <param name="nonce_key">密匙 + 随机字符串</param>
        /// <returns></returns>
        public static string HmacSHA256_Base64_2(string base_string, string nonce_key)
        {
            byte[] hash;
            using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(nonce_key)))
            {
                hash = mac.ComputeHash(Encoding.UTF8.GetBytes(base_string));
            }
            hash = Encoding.UTF8.GetBytes(hash.ToHexString());
            return Convert.ToBase64String(hash);
        }
    }
}
