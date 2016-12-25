using System;

namespace Server
{
    static class RequestHandler
    {
        public static Response HandleRequest(Request request)
        {
            Response response = new Response();
            BaseController controller = Routes.Resolve(request);

            switch (request.Type)
            {
                case RequestType.DELETE:
                    response = controller.Delete(request);
                    break;

                case RequestType.GET:
                    response = controller.Get(request);
                    break;

                case RequestType.POST:
                    response = controller.Post(request);
                    break;

                case RequestType.PUT:
                    response = controller.Put(request);
                    break;

                case RequestType.UPDATE:
                    response = controller.Update(request);
                    break;

                default:
                    throw new Exception("Request type not supported or recognized.");
            }

            return response;
        }
    }
}
