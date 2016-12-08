using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    class Response
    {
        private Dictionary<string, string> headers = new Dictionary<string, string>();

        public string Version { get; set; }

        public StatusCode StatusCode { get; set; }

        public byte[] Data{ get; set; }

        public void AddHeader(string key, string value)
        {
            headers.Add(key, value);
        }

        public byte[] ToByteArray()
        {
            byte[] header = ConstructHeader();
            return header.Concat(Data).ToArray();
        }

        private byte[] ConstructHeader()
        {
            string header = string.Empty;
            string newLine = "\r\n";

            header += string.Format("{0} {1}", Version, StatusCodeToString(StatusCode));
            header += newLine;
            
            foreach(KeyValuePair<string, string> kvp in headers)
            {
                header += string.Format("{0}: {1}", kvp.Key, kvp.Value);
                header += newLine;
            }

            if (!headers.ContainsKey("Content-Length"))
            {
                header += string.Format("Content-Length: {0}", Data.Length);
                header += newLine;
            }

            header += newLine;

            return Encoding.ASCII.GetBytes(header);
        }

        private string StatusCodeToString(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.OK:
                    return "200 OK";

                case StatusCode.Created:
                    return "201 Created";

                case StatusCode.NoContent:
                    return "204 No Content";

                case StatusCode.BadRequest:
                    return "400 Bad Request";

                case StatusCode.Unauthorized:
                    return "401 Unauthorized";

                case StatusCode.Forbidden:
                    return "403 Forbidden";

                case StatusCode.NotFound:
                    return "404 Not Found";

                case StatusCode.Conflict:
                    return "409 Conflict";

                case StatusCode.InternalServerError:
                    return "500 Internal Server Error";

                default:
                    return "Status Code Not Recognized";
            }
        }
    }
}
