namespace Food.Web
{
    public static class StartingDetails
    {
        public static string ProductAPIbase { get; set; }
        public static string ShoppingCartAPIAPIbase { get; set; }
        public static string CouponAPIAPIbase { get; set; }
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public enum ContentType
        {
            Json,
            MultipartFormData,
        }
    }
}
