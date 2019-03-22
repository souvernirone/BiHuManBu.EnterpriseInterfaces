using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Compress
{

    //public static class WebApiConfig
    //{
    //    public static void Register(HttpConfiguration config)
    //    {
    //        
    //    }
    //}
    /// <summary>
    /// 解压缩
    /// </summary>
    public class DecompressionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
       CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post)
            {
                bool isGzip = request.Content.Headers.ContentEncoding.Contains("gzip");
                bool isDeflate = !isGzip && request.Content.Headers.ContentEncoding.Contains("deflate");

                if (isGzip || isDeflate)
                {
                    Stream decompressedStream = new MemoryStream();

                    if (isGzip)
                    {
                        using (var gzipStream = new GZipStream(await request.Content.ReadAsStreamAsync(),
                            CompressionMode.Decompress))
                        {
                            await gzipStream.CopyToAsync(decompressedStream);
                        }
                    }
                    else if (isDeflate)
                    {

                        using (var gzipStream = new DeflateStream(await request.Content.ReadAsStreamAsync(),
                            CompressionMode.Decompress))
                        {
                            await gzipStream.CopyToAsync(decompressedStream);
                        }
                    }

                    decompressedStream.Seek(0, SeekOrigin.Begin);

                    var originContent = request.Content;
                    request.Content = new StreamContent(decompressedStream);

                    foreach (var header in originContent.Headers)
                    {
                        request.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
