namespace Pyromaniac.DTOs
{
    public class ResponseEnvelope
    {
        public int Status { get; init; } = ResponseCodeGenerator.GenerateCode();

        public string Message { get; init; } = "Pyromaniac Burned This Response";

        public dynamic? Data { get; set; }
    }

    public static class ResponseCodeGenerator
    {
        private static readonly List<int> ResponseCodes = new() { 400, 401, 403, 404, 405, 406, 408, 415, 500, 501, 502, 503, 504 };
        public static int GenerateCode()
        {
            return ResponseCodes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }
    }
}
