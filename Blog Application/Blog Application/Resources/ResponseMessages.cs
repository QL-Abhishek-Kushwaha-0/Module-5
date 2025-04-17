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
        public const string INVALID_AUTHOR = "Invalid Author!!!";
        public const string INVALID_LOGIN = "Please Login to Continue!!!";


        // Category Related Messages

        public const string NO_CATEGORIES = "No Category Found!!!";
        public const string CATEGORIES_FETCHED = "Categories Fetched Successfully...";
        public const string NO_CATEGORY = "No such Category Found!!!";
        public const string CATEGORY_FETCHED = "Category Fetched Successfully...";
        public const string CATEGORY_EXISTS = "Category already exists!!!";
        public const string CATEGORY_CREATED = "Category created Successfully...";
        public const string NO_CATEGORY_CREATED = "No Categories are created yet!!!";
        public const string CATEGORY_UPDATED = "Category updated Successfully...";
        public const string CATEGORY_CONFLICT = "Cannot Delete Category created by other Author!!!";
        public const string CATEGORY_DELETE = "Category Deleted Successfully...";

        // Post Related Messages

        public const string POSTS_FETCHED = "Posts Fetched Successfully...";
        public const string POST_FETCHED = "Post Fetched Successfully...";
        public const string NO_POST = "No such Post Found!!!";
        public const string NO_POSTS = "No Posts Found!!!";
        public const string NO_PUBLISHED_POSTS = "No Posts are published yet!!!";
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

        // User Interaction Messages
        public const string LOGIN_TO_INTERACT = "Please Login to Continue!!!!";
        public const string LIKE_CONFLICT = "Post has already been Liked!!!!";
        public const string UNLIKE_CONFLICT = "Post has not yet been Liked!!!!";
        public const string LIKE_SUCCESS = "Post Liked Successfully...";
        public const string UNLIKE_SUCCESS = "Post Unliked Successfully...";
        public const string COMMENT_SUCCESS = "Commented Successfully...";
        public const string COMMENT_DELETE_SUCCESS = "Comment Deleted Successfully...";
        public const string COMMENT_NOT_FOUND = "No such Comment Found!!!";
        public const string COMMENT_CONFLICT = "Cannot Delete Comment posted by others!!!!";
        public const string COMMENTS_FETCHED = "Comments Fetched Successfully...";
        public const string COMMENTS_NOT_FOUND = "No Comments Found for this Post!!!";


        // Subscription Related Messages
        public const string SUBSCRIBERS_FETCHED = "Subscribers Fetched Successfully...";
        public const string SUBSCRIPTIONS_FETCHED = "Subscriptions Fetched Successfully...";
        public const string SUBSCRIBE_SELF_CONFLICT = "Cannot Subscribe to yourself!!!";
        public const string SUBSCRIBE_SUCCESS = "Subscribed Successfully...";
        public const string UNSUBSCRIBE_SUCCESS = "Unsubscribed Successfully...";
        public const string INVALID_SUBSCRIBE = "Can only Subscribe to Authors!!!";
        public const string SUBSCRIBE_CONFLICT = "Already Subscribed to this Author!!!";
        public const string UNSUBSCRIBE_CONFLICT = "Not yet Subscribed to this Author!!!";

        public const string SUBSCRIBER_CONFLICT = "Invalid Request!! Viewers cannot have subscribers!!!";
        public const string SUBSCRIBER_FETCHED = "Subscribers Fetched Successfully...";

        public const string SUBSCRIPTION_ACCESS_CONFLICT = "Cannot Access other user's subscriptions!!!";
        public const string SUBSCRIPTION_FETCHED = "Subscriptions Fetched Successfully...";
        public const string SUBSCRIPTION_CONFLICT = "You haven't Subscribed to any Author!!!";

        // Image Upload related messages
        public const string NO_IMAGE = "No Image Uploaded!!!";
        public const string IMAGE_UPLOADED = "Image Uploaded Successfully...";
        public const string INVALID_IMAGE = "Invalid Image Format!!!";

        // DTO related messages
        public const string MIN_LENGTH = "Should be at least 3 chalacters long!!!";
        public const string MAX_LENGTH = "Should not exceed 20 characters!!!";
        public const string MAX_NAME_LENGTH = "Should not be more that 50 characters!!!";
        public const string INVALID_EMAIL = "Enter a Valid Email Address!!!!";
        public const string INVALID_PASSWORD = "Password should be at least 8 characters long!!!";
        public const string INVALID_URL = "Invalid URL!!!";

    }
}
