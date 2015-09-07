using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Utils
{
    public class HashUtils
    {
        public static byte[] ComputeMd5(Stream stream)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(stream);
        }

        /// <summary>
        /// 计算ETag值(使用MD5)
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static String ComputeETagByMd5(Stream stream)
        {
            return Convert.ToBase64String(ComputeMd5(stream));
        }
    }
}
