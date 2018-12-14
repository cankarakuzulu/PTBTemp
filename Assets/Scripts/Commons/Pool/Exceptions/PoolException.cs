using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nopact.Commons.Pool.Exceptions
{
    public class PoolException :ApplicationException
    {
        public PoolException()
        {
        }

        public PoolException( string message ) : base( message ) 
        {
            
        }

        public PoolException( string message, Exception innerException ) : base( message, innerException )
        {
        }

        protected PoolException( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
        }
    }
}
