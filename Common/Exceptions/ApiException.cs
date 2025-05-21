using System;

namespace EmployeeHub.Common.Exceptions
{
    public class ApiException(string message, int statusCode = 500) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
    }

    public class NotFoundException(string message) : ApiException(message, 404)
    {
    }

    public class BadRequestException(string message) : ApiException(message, 400)
    {
    }

    public class UnauthorizedException(string message) : ApiException(message, 401)
    {
    }

    public class AccessDeniedException(string message) : ApiException(message, 403)
    {
    }

    public class ConflictException(string message) : ApiException(message, 409)
    {
    }

    public class ValidationException(string message) : ApiException(message, 422)
    {
    }

    public class TooManyRequestsException(string message) : ApiException(message, 429)
    {
    }
}