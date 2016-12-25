using System;

namespace Server
{
    class InvalidRequestException : Exception
    {
        private static readonly string baseMessage = "Invalid request to web server";

        public InvalidRequestException()
            : base(GetMessage(null))
        {
        }

        public InvalidRequestException(string message)
            : base(GetMessage(message))
        {
        }

        public InvalidRequestException(string message, Exception innerException)
            : base(GetMessage(message), innerException)
        {
        }

        private static string GetMessage(string message = null)
        {
            return string.IsNullOrEmpty(message) ? baseMessage : string.Format("{0} : {1}", baseMessage, message);
        }
    }
}
