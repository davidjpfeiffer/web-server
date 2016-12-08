using System;
using System.IO;
using System.Text;

namespace Server
{
    static class Controller
    {
        public static byte[] HandleRequest(string rawHttpRequest)
        {
            Request request = new Request(rawHttpRequest);

            switch (request.Type)
            {
                case RequestType.GET:
                    return Get(request);

                case RequestType.PUT:
                    return Put(request);

                case RequestType.POST:
                    return Post(request);

                case RequestType.UPDATE:
                    return Update(request);

                case RequestType.DELETE:
                    return Delete(request);

                default:
                    throw new Exception("Request type not recognized");
            }
        }

        private static byte[] Get(Request request)
        {
            Response response = new Response();

            switch (request.Url.ToLower())
            {
                case "/":
                case "/index.html":
                    response.Version = "HTTP/1.1";
                    response.Data = GetTextResource("../../Resources/index.html");
                    response.AddHeader("Content-Type: ", "text/html");
                    response.StatusCode = StatusCode.OK;
                    break;

                case "/about":
                case "/about.html":
                    response.Version = "HTTP/1.1";
                    response.Data = GetTextResource("../../Resources/about.html");
                    response.AddHeader("Content-Type: ", "text/html");
                    response.StatusCode = StatusCode.OK;
                    break;

                case "/funny":
                case "/funny.gif":
                    response.Version = "HTTP/1.1";
                    response.Data = GetFileResource("../../Resources/funny.gif");
                    response.AddHeader("Content-Type: ", "image/gif");
                    response.StatusCode = StatusCode.OK;
                    break;

                default:
                    response.Version = "HTTP/1.1";
                    response.Data = GetTextResource("../../Resources/404.html");
                    response.AddHeader("Content-Type: ", "text/html");
                    response.StatusCode = StatusCode.NotFound;
                    break;
            }

            return response.ToByteArray();
        }

        private static byte[] Put(Request request)
        {
            throw new NotImplementedException();
        }

        private static byte[] Post(Request request)
        {
            throw new NotImplementedException();
        }

        private static byte[] Update(Request request)
        {
            throw new NotImplementedException();
        }

        private static byte[] Delete(Request request)
        {
            throw new NotImplementedException();
        }

        private static byte[] GetTextResource(string resourcePath)
        {
            return Encoding.ASCII.GetBytes(File.ReadAllText(resourcePath));
        }

        private static byte[] GetFileResource(string resourcePath)
        {
            return File.ReadAllBytes(resourcePath);
        }
    }
}
