using Eut.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Eut.Service.MSSharePoint
{
    public class EutSharePointException: ApplicationException, IEutException
    {
        public EutSharePointException(): base() { }
        public EutSharePointException(string message) : base(message) { }
        public EutSharePointException(string message, Exception innerException) : base(message, innerException) { }
        protected EutSharePointException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
