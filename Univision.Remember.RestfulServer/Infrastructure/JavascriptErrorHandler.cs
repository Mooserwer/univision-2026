using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Remember.RestfulServer.Infrastructure
{
    public class JavascriptErrorHandler
    {
        [Serializable]
        public class JavaScriptErrorException : Exception
        {
            public JavaScriptErrorException(string message) : base(message)
            {
            }
        }
    }
}