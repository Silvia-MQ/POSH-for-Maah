using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POSH.sys.exceptions
{
    public class AttributeException : Exception
    {
        public AttributeException() {}
        public AttributeException(string message) : base(message) {}
        public AttributeException(string message, System.Exception inner) : base(message,inner) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected AttributeException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info,context) {}
    }
}
