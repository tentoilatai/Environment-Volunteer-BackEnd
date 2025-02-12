using EnvironmentVolunteer.Core.Enums;
using EnvironmentVolunteer.Core.Extensions;
using System;

namespace EnvironmentVolunteer.Core.Exceptions
{
    public class ErrorException : Exception
    {
        public StatusCodeEnum StatusCode { get; set; }
        public object Data { get; set; }

        public ErrorException(StatusCodeEnum statusCode) : base(statusCode.GetDescription())
        {
            StatusCode = statusCode;
        }

        public ErrorException(StatusCodeEnum statusCode, object data) : base(statusCode.GetDescription())
        {
            StatusCode = statusCode;
            Data = data;
        }

        public ErrorException(StatusCodeEnum statusCode, string customMessage) : base(customMessage)
        {
            StatusCode = statusCode;
        }

        public ErrorException(string message) : base(message) { }
    }
}
