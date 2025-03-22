namespace Blog_Application.Utils
{
    public class GlobalException : Exception
    {
        public GlobalException()
        {
            throw new Exception("Something Went Wrong!!!!");
        }
        public GlobalException(string message)
        {
            throw new Exception(message);
        }
    }
}
