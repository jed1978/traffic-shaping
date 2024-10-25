using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.SignalR;

namespace RequestController.Hubs
{
    public class RequestHub : Hub
    {
        private readonly IRedisService _redisService;

        public RequestHub(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task SendRequest(string requestId, object requestData)
        {
            await _redisService.EnqueueRequestAsync(new RequestMessage
            {
                RequestId = requestId,
                ConnectionId = Context.ConnectionId,
                Data = requestData
            });
        }
    }
}