using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using uhttpsharp;

namespace AudioBook2Podcast
{
    public class UhttpShartServer
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

        public void Stop()
        {
            HttpServer.Instance.Stop();
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
        }

        private string GetContentType(string path)
        {

            return MIMEAssistant.GetMIMEType(Path.GetFileName(path), DefaultMimeType);

        }
        public override HttpResponse Handle(HttpRequest httpRequest)
        {
            var httpRoot = Path.GetFullPath(HttpRootDirectory ?? ".");
            var requestPath = Uri.UnescapeDataString(httpRequest.Uri.AbsolutePath.TrimStart('/'));
            var path = Path.GetFullPath(Path.Combine(httpRoot, requestPath));
            if (!File.Exists(path))
                return null;
            return new HttpResponse(GetContentType(path), File.OpenRead(path));
        }
    }

}
