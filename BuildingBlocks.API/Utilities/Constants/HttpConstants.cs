namespace BuildingBlocks.API.Utilities.Constants;

public static class HttpConstants
{
    internal static class StatusCodes
    {
        public const int Ok = 200;
        public const int Created = 201;
        public const int NoContent = 204;
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int Conflict = 409;
        public const int UnprocessableEntity = 422;
        public const int TooManyRequests = 429;
        public const int InternalServerError = 500;
    }

    internal static class ContentTypes
    {
        public const string Json = "application/json";
        public const string Xml = "application/xml";
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";
        public const string MultipartFormData = "multipart/form-data";
        public const string TextPlain = "text/plain";
        public const string Html = "text/html";
    }

    internal static class Methods
    {
        public const string Get = "GET";
        public const string Post = "POST";
        public const string Put = "PUT";
        public const string Delete = "DELETE";
        public const string Patch = "PATCH";
        public const string Options = "OPTIONS";
        public const string Head = "HEAD";
    }
}