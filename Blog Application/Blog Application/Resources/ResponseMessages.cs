namespace Blog_Application.Resources
{
    public static class ResponseMessages
    {
        // User Related Messages

        public const string USER_EXISTS = "User already exists!!!!";
        public const string USER_REGISTERED = "User Registered Successfully!!!!";
        public const string INVALID_CREDENTIALS = "Invalid Email or Password!!!!";
        public const string LOGIN_SUCCESS = "Logged In Successfully....";

        public const string INVALID_GUID = "Invalid User!!!";

        // Category Related Messages

        public const string NO_CATEGORIES = "No Category Found!!!";
        public const string CATEGORIES_FETCHED = "Categories Fetched Successfully...";
        public const string NO_CATEGORY = "No such Category Found!!!";
        public const string CATEGORY_FETCHED = "Category Fetched Successfully...";
        public const string CATEGORY_EXISTS = "Category already exists!!!";
        public const string CATEGORY_CREATED = "Category created Successfully...";
        public const string CATEGORY_UPDATED = "Category updated Successfully...";
        public const string CATEGORY_CONFLICT = "Cannot Delete Category created by other Author!!!";
        public const string CATEGORY_DELETE = "Category Deleted Successfully...";

        // Post Related Messages

        public const string POSTS_FETCHED = "Posts Fetched Successfully...";
        public const string POST_FETCHED = "Post Fetched Successfully...";
        public const string NO_POST = "No such Post Found!!!";
        public const string POST_UPDATED = "Post Updated Successfully...";
        public const string POST_CREATE_CONFLICT = "Invalid Request for Post Creation (Either Post already exists or Wrong Category!!!!";
        public const string POST_CREATED = "Post Created Successfully...";
        public const string PUBLISH_CONFLICT = "Only Post owners can publish the post!!!";
        public const string PUBLISHED_POST_CONFLICT = "Post has already been published!!!";
        public const string POST_PUBLISHED = "Post Published Successfully...";
        public const string POST_UNPUBLISHED = "Post Unpublished Successfully...";
        public const string UNPUBLISH_CONFLICT = "Onlu Post owners can unpublish the post!!!";
        public const string UNPUBLISHED_POST_CONFLICT = "Post has not yet been published!!!";
        public const string POST_DELETE_CONFLICT = "Cannot delete Posts created by other Author!!!";
        public const string POST_DELETE = "Post Deleted Successfully...";
    }
}
