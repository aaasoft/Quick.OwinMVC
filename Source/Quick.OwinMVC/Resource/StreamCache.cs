using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Resource
{
    public class StreamCache
    {
        private byte[] buffer = null;
        /// <summary>
        /// 实际大小
        /// </summary>
        public int ActualSize { get; private set; }

        public Stream GetStream()
        {
            var ms = new MemoryStream(buffer);
            var gzStream = new GZipStream(ms, CompressionMode.Decompress);
            return gzStream;
        }

        public StreamCache(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                using (var gzStream = new GZipStream(ms, CompressionMode.Compress))
                    stream.CopyTo(gzStream);
                buffer = ms.ToArray();
                ActualSize = buffer.Length;
            }
        }
    }
}
