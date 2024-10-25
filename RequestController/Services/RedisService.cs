using System.Text.Json;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.SignalR;
using RequestController.Hubs;
using StackExchange.Redis;

namespace RequestController.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IHubContext<RequestHub> _hubContext;

        public RedisService(IConnectionMultiplexer redis, IHubContext<RequestHub> hubContext)
        {
            _redis = redis;
            _hubContext = hubContext;
        }

        public async Task EnqueueRequestAsync(RequestMessage requestMessage)
        {
            var db = _redis.GetDatabase();
            var message = JsonSerializer.Serialize(requestMessage);

            // 將request放入queue（List）
            await db.ListRightPushAsync("RequestQueue", message);
        }

        public async Task SubscribeToResponsesAsync()
        {
            var subscriber = _redis.GetSubscriber();

            await subscriber.SubscribeAsync("ResponseChannel", async (channel, message) =>
            {
                var responseMessage = JsonSerializer.Deserialize<ResponseMessage>(message);

                // 透過SignalR發送結果到Client
                await _hubContext.Clients.Client(responseMessage.ConnectionId)
                    .SendAsync("ReceiveResponse", responseMessage.RequestId, responseMessage.Data);
            });
        }
    }
}