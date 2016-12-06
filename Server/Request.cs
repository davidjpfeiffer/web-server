using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    class Request
    {
        private readonly List<string> requestElements;

        public Request(string httpRequest)
        {
            requestElements = httpRequest.Split(' ').ToList();

            if (requestElements.Count < 2)
            {
                throw new Exception("Invalid argument passed to Request constructor");
            }
        }

        public override string ToString()
        {
            return $@"{RequestType} {Url}";
        }

        public string Url
        {
            get
            {
                return requestElements[1];
            }
        }

        public RequestType RequestType
        {
            get
            {
                switch (requestElements[0])
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
}
