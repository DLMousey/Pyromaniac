namespace Pyromaniac.DTOs.Outbound
{
    public class ResponseOutputDto
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = "Pyromaniac Burned This Response";

        public InvokationChangeOutputDto? Data { get; set; }
    }
}
