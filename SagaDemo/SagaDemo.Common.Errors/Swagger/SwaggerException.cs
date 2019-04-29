using System;
using System.Collections.Generic;

namespace SagaDemo.Common.Errors.Swagger
{
    // This class is based on what the NSwag tool would generate. The generated code has been copied here and every API client generated should have their 'generateExceptionClasses'
    // properties generated and the namespace of this class added. This is needed because then this exception can be shared among different API clients and therefore it is possible
    // to create exception filters for it in the shared ASP.NET Core library.
    public class SwaggerException : Exception
    {
        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public Dictionary<string, IEnumerable<string>> Headers { get; private set; }

        public SwaggerException(string message, int statusCode, string response, Dictionary<string, IEnumerable<string>> headers, Exception innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + response.Substring(0, response.Length >= 512 ? 512 : response.Length), innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }
}