using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLib01
{
    public static class NonceHelper
    {

        /// <summary>
        /// 创建5字的请求随机key<br/>
        /// 2023-1-11 Ciaran
        /// </summary>
        /// <returns></returns>
        public static string Nonce5Char()
        {
            // 2023-1-11 Ciaran 生成新的Guid, 并通过guid的字节值计算得出对应随机字符
            string rdKey = new string(Guid.NewGuid()
                    .ToByteArray()
                    .Period(3) // 16个字节 , 每3个为一组, 有6组
                    .Take(5) // 取前5个
                    .Select(list => ((list[2] << 16) + (list[1] << 8) + list[0]) % 36) // 得出当个字节映射
                    .Select(i => i < 26 ? (char)('a' + i) : (char)('0' + i - 26)) // 映射为字符
                    .ToArray()) // 字符数组 转 字符串
                ;
            return rdKey;
        }

        /// <summary>
        /// newGuid.SubString(start, len);
        /// </summary>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string SubGuid(int start = 2, int len = 10) => Guid.NewGuid().ToString().Replace("-", "").Substring(start, len);
    }
}
