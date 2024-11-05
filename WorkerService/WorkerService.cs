using System.Text.Json;
using Common.Models;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;

        public Worker(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _redis.GetDatabase();
            var subscriber = _redis.GetSubscriber();

            while (!stoppingToken.IsCancellationRequested)
            {
                // 從請求隊列中拉取消息
                var message = await db.ListLeftPopAsync("RequestQueue");

                if (message.IsNullOrEmpty)
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                var requestMessage = JsonSerializer.Deserialize<RequestMessage>(message);

                // 處理請求
                var result = await ProcessRequestAsync(requestMessage.Data);

                // 構建響應消息
                var responseMessage = new ResponseMessage
                {
                    RequestId = requestMessage.RequestId,
                    ConnectionId = requestMessage.ConnectionId,
                    Data = result
                };

                // 將結果發佈到 Redis
                await subscriber.PublishAsync("ResponseChannel", JsonSerializer.Serialize(responseMessage));
            }
        }

        private Task<object> ProcessRequestAsync(object requestData)
        {
            // 執行業務邏輯
            Console.WriteLine($"Processing request at {DateTime.Now}");
            
            // 模擬處理時間
            Thread.Sleep(100);

            // 返回處理結果
            return Task.FromResult<object>(new { Success = true, Data = "Processed data" });
        }
    }
}