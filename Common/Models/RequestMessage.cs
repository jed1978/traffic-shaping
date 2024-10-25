namespace Common.Models;

public class RequestMessage
{
    public string RequestId { get; set; }
    public string ConnectionId { get; set; }
    public object Data { get; set; }
}