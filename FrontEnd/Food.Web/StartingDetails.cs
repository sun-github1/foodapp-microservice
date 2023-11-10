﻿namespace Food.Web
{
    public static class StartingDetails
    {
        public static string ProductAPIbase { get; set; }

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