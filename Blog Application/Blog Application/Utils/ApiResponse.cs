namespace Blog_Application.Utils
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public object Data { get; set; }

        public ApiResponse(bool success, int statusCode, string message)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = new object();
        }

        public ApiResponse(bool success, int statusCode, string message, object data)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

    }
}
