using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using uhttpsharp;

namespace AudioBook2Podcast
{
    class UhttpShartServer
    {
        public string path { get; set; }
        public int port { get; set; }


        public void Start()
        {
            HttpServer.Instance.Port = port;
            FileHandler.HttpRootDirectory = path;
            IndexHandler.Resp = File.ReadAllText(path + "/podcast.xml");
            HttpServer.Instance.StartUp();
        }
    }

    [HttpRequestHandlerAttributes("")]
    public class IndexHandler : HttpRequestHandler
    {
        public static string Resp { get; set; }
        public override HttpResponse Handle(HttpRequest httpRequest)
        {
            MemoryStream msResp = new MemoryStream(Encoding.UTF8.GetBytes(Resp));
            return new HttpResponse("application/rss+xml", msResp);
            
        }
    }


    [HttpRequestHandlerAttributes("*")]
    public class FileHandler : HttpRequestHandler
    {
        public static string DefaultMimeType { get; set; }
        public static string HttpRootDirectory { get; set; }
        public static IDictionary<string, string> MimeTypes { get; private set; }

        static FileHandler()
        {
            DefaultMimeType = "application/rss+xml";
            MimeTypes = new Dictionary<string, string>
                            {
                                {".css", "text/css"},
                                {".gif", "image/gif"},
                                {".htm", "text/html"},
                                {".html", "text/html"},
                                {".jpg", "image/jpeg"},
                                {".js", "application/javascript"},
                                {".png", "image/png"},
                                {".xml", "application/rss+xml"},
                                {".mp3", "audio/mpeg"},
                                {".wma", "audio/x-ms-wma"}
                            };
        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path) ?? "";
            if (MimeTypes.ContainsKey(extension))
                return MimeTypes[extension];
            return DefaultMimeType;
        }
        public override HttpResponse Handle(HttpRequest httpRequest)
        {
            var httpRoot = Path.GetFullPath(HttpRootDirectory ?? ".");
            var requestPath = httpRequest.Uri.AbsolutePath.TrimStart('/');
            var path = Path.GetFullPath(Path.Combine(httpRoot, requestPath));
            if (!File.Exists(path))
                return null;
            return new HttpResponse(GetContentType(path), File.OpenRead(path));
        }
    }

}
