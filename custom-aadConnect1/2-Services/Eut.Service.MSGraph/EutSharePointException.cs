using Eut.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Eut.Service.MSGraph
{
    public class EutGraphException: ApplicationException, IEutException
    {
        public EutGraphException(): base() { }
        public EutGraphException(string message) : base(message) { }
        public EutGraphException(string message, Exception innerException) : base(message, innerException) { }
        protected EutGraphException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
