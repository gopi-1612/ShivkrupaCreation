namespace ShivkrupaCreation.API.DTOs.Common
{
    /// <summary>
    /// Uniform envelope for every API response so the Angular frontend
    /// can rely on one shape for success, data, and error messages.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors };
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
