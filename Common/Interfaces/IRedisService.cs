using Common.Models;

namespace Common.Interfaces;

public interface IRedisService
{
    Task SubscribeToResponsesAsync();
    Task EnqueueRequestAsync(RequestMessage requestMessage);
}