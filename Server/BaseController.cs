using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    abstract class BaseController
    {
        public abstract Response Delete(Request request);

        public abstract Response Get(Request request);

        public abstract Response Post(Request request);

        public abstract Response Put(Request request);

        public abstract Response Update(Request request);

        protected static byte[] GetResource(string resourcePath)
        {
            return Encoding.ASCII.GetBytes(File.ReadAllText(resourcePath));
        }
    }
}
