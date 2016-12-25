using System.Collections.Generic;

namespace Server
{
    static class Routes
    {
        private static Dictionary<string, BaseController> routes = GetRoutes();

        public static BaseController Resolve(Request request)
        {
            string requestUrl = request.Url.ToLower();

            BaseController controller = new NotFoundController();

            if (routes.ContainsKey(requestUrl))
            {
                controller = routes[requestUrl];
            }

            return controller;
        }

        private static Dictionary<string, BaseController> GetRoutes()
        {
            Dictionary<string, BaseController> routes = new Dictionary<string, BaseController>();

            routes.Add("/", new HomeController());
            routes.Add("/about", new AboutController());

            return routes;
        }
    }
}
