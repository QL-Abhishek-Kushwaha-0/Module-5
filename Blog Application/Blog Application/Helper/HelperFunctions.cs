using Blog_Application.Middlewares;

namespace Blog_Application.Helper
{
    public class HelperFunctions
    {
        public static Guid GetGuid(string guid)
        {
            if(guid.Length == 0) return Guid.Empty;
            return Guid.TryParse(guid, out Guid resId) ? resId : Guid.Empty;
        }

    }
}
