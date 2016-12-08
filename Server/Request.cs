using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    class Request
    {
        public Request(string rawHttpRequest)
        {
            try
            {
                List<string> requestLines = rawHttpRequest
                    .Replace('\r', ' ')
                    .Trim()
                    .Split('\n')
                    .Select(i => i.Trim())
                    .ToList();

                string[] firstRequestLine = requestLines.First().Split(' ');
                Type = GetRequestTypeFromString(firstRequestLine[0]);
                Url = firstRequestLine[1];
                Version = firstRequestLine[2];

                requestLines.RemoveAt(0);

                Headers = requestLines.ToDictionary(
                    keySelector: i => i.Substring(0, i.IndexOf(':')),
                    elementSelector: i => i.Substring(i.IndexOf(' ') + 1, i.Length - i.IndexOf(' ') - 1));
            }
            catch(Exception exception)
            {
                throw new Exception("Invalid argument passed to Request constructor", exception);
            }
        }

        public override string ToString()
        {
            return $@"{Type} {Url} {Version}";
        }

        public Dictionary<string, string> Headers { get; private set; }

        public string Url { get; private set; }

        public RequestType Type { get; private set; }

        public string Version { get; private set; }

        private RequestType GetRequestTypeFromString(string requestType)
        {
            switch (requestType)
            {
                case "GET":
                    return RequestType.GET;

                case "UPDATE":
                    return RequestType.UPDATE;

                case "POST":
                    return RequestType.POST;

                case "PUT":
                    return RequestType.PUT;

                case "DELETE":
                    return RequestType.DELETE;

                default:
                    return RequestType.INVALID;

            }
        }
    }
}
