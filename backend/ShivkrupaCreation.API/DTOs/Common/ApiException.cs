namespace ShivkrupaCreation.API.DTOs.Common
{
    /// <summary>
    /// Throw this from services/controllers for expected, user-facing errors
    /// (not found, conflict, forbidden, etc). The centralized exception
    /// middleware maps it straight to the given HTTP status + message.
    /// </summary>
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public List<string>? Errors { get; }

        public ApiException(string message, int statusCode = 400, List<string>? errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }

        public static ApiException NotFound(string message) => new(message, 404);
        public static ApiException Conflict(string message) => new(message, 409);
        public static ApiException Unauthorized(string message) => new(message, 401);
        public static ApiException Forbidden(string message) => new(message, 403);
        public static ApiException BadRequest(string message, List<string>? errors = null) => new(message, 400, errors);
    }
}
