using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QI.WikiScraping.Api.Application.Exceptions
{
    /// <summary>
    /// Will be thrown if there is an intent to test the App with an Url distinct than wikipedia site
    /// </summary>
    public class QualIdInvalidTestException : Exception
    {


        public QualIdInvalidTestException()
        { }

        public QualIdInvalidTestException(string message)
            : base(message)
        { }

        public QualIdInvalidTestException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected QualIdInvalidTestException(SerializationInfo info, StreamingContext context) :
           base(info, context)
        {

        }
    }
}
