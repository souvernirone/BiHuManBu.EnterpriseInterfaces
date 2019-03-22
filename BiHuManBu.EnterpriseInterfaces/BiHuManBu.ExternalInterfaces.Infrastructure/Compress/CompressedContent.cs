using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Compress
{
    //    var json = JsonConvert.SerializeObject(bookmark);
    //var content = new CompressedContent(
    //    new StringContent(json, Encoding.UTF8, "application/json"), 
    //    CompressionMethod.GZip);
    //var response = await _httpClient.PostAsync("/api/bookmarks", content);
    public enum CompressionMethod
    {
        GZip = 1,
        Deflate = 2
    }
    /// <summary>
    /// 压缩请求内容
    /// </summary>
    public  class CompressedContent:HttpContent
    {
        private readonly HttpContent _originalContent;
        private readonly CompressionMethod _compressionMethod;

        public CompressedContent(HttpContent content, CompressionMethod compressionMethod)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            _originalContent = content;
            _compressionMethod = compressionMethod;

            foreach (KeyValuePair<string, IEnumerable<string>> header in _originalContent.Headers)
            {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            Headers.ContentEncoding.Add(_compressionMethod.ToString().ToLowerInvariant());
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }

        protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (_compressionMethod == CompressionMethod.GZip)
            {
                using (var gzipStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
                {
                    await _originalContent.CopyToAsync(gzipStream);
                }
            }
            else if (_compressionMethod == CompressionMethod.Deflate)
            {
                using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress, leaveOpen: true))
                {
                    await _originalContent.CopyToAsync(deflateStream);
                }
            }
        }
    }
}
