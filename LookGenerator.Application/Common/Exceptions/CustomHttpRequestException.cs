using System.Net;

namespace LookGenerator.Application.Common.Exceptions ;

    public class CustomHttpRequestException(HttpStatusCode statusCode, string message) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }