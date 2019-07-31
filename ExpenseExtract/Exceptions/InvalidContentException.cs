using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ExpenseExtract.Exceptions
{
    [Serializable]
    public class InvalidContentException : Exception
    {
        public InvalidContentException()
        {
        }

        public InvalidContentException(string message)
            : base(message)
        {
        }

        public InvalidContentException(string message, Exception inner)
            : base(message, inner)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected InvalidContentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
