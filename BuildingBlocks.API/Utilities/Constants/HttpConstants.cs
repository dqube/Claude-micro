namespace BuildingBlocks.API.Utilities.Constants;

public static class HttpConstants
{
    public static class StatusCodes
    {
        // 2xx Success
        public const int Ok = 200;
        public const int Created = 201;
        public const int Accepted = 202;
        public const int NoContent = 204;

        // 3xx Redirection
        public const int NotModified = 304;

        // 4xx Client Error
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int MethodNotAllowed = 405;
        public const int NotAcceptable = 406;
        public const int Conflict = 409;
        public const int Gone = 410;
        public const int PreconditionFailed = 412;
        public const int UnsupportedMediaType = 415;
        public const int UnprocessableEntity = 422;
        public const int TooManyRequests = 429;

        // 5xx Server Error
        public const int InternalServerError = 500;
        public const int NotImplemented = 501;
        public const int BadGateway = 502;
        public const int ServiceUnavailable = 503;
        public const int GatewayTimeout = 504;
    }

    public static class Methods
    {
        public const string Get = "GET";
        public const string Post = "POST";
        public const string Put = "PUT";
        public const string Delete = "DELETE";
        public const string Patch = "PATCH";
        public const string Head = "HEAD";
        public const string Options = "OPTIONS";
        public const string Trace = "TRACE";
    }

    public static class MediaTypes
    {
        public const string Json = "application/json";
        public const string Xml = "application/xml";
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";
        public const string MultipartFormData = "multipart/form-data";
        public const string TextPlain = "text/plain";
        public const string TextHtml = "text/html";
        public const string TextCsv = "text/csv";
        public const string ApplicationPdf = "application/pdf";
        public const string ApplicationOctetStream = "application/octet-stream";
    }

    public static class Encodings
    {
        public const string Gzip = "gzip";
        public const string Deflate = "deflate";
        public const string Brotli = "br";
        public const string Identity = "identity";
    }

    public static class CacheControl
    {
        public const string NoCache = "no-cache";
        public const string NoStore = "no-store";
        public const string MaxAge = "max-age";
        public const string MustRevalidate = "must-revalidate";
        public const string Private = "private";
        public const string Public = "public";
    }
}