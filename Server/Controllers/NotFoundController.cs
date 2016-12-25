using System;

namespace Server
{
    class NotFoundController : BaseController
    {
        public override Response Delete(Request request)
        {
            throw new InvalidRequestException();
        }

        public override Response Get(Request request)
        {
            Response response = new Response();

            response.Version = "HTTP/1.1";
            response.Data = GetResource("../../Resources/not-found.html");
            response.AddHeader("Content-Type: ", "text/html");
            response.StatusCode = StatusCode.NotFound;

            return response;
        }

        public override Response Post(Request request)
        {
            throw new InvalidRequestException();
        }

        public override Response Put(Request request)
        {
            throw new InvalidRequestException();
        }

        public override Response Update(Request request)
        {
            throw new InvalidRequestException();
        }
    }
}
