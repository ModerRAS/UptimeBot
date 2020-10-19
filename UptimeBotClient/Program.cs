using Microsoft.Extensions.Hosting;
using Coravel;
using System;
using Coravel.Invocable;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace UptimeBotClient {
    public class Env {
        public static readonly string ServerAddress = Environment.GetEnvironmentVariable("ServerAddress");
        public static readonly string NodeName = Environment.GetEnvironmentVariable("NodeName");
        public static readonly string Token = Environment.GetEnvironmentVariable("Token");
    }
    class Data {
        public string NodeName { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsUp { get; set; }
    }
    class Program {
        public static async Task<string> CrawlString(string url, HttpClient client) {
            var tick = 5;
            while (tick > 0) {
                try {
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Content = new StringContent(
                        JsonConvert.SerializeObject(
                            new Data {
                                NodeName = Env.NodeName,
                                DateTime = DateTime.UtcNow,
                                IsUp = true
                            }));
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
                    var response = await client.SendAsync(request).ConfigureAwait(true);
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                } catch (HttpRequestException) {
                    tick--;
                }
            }
            throw new HttpRequestException();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(service => {
                    service.AddScheduler();
                });
        static void Main(string[] args) {
            IHost host = CreateHostBuilder(args).Build();
            host.Services.UseScheduler(scheduler => {
                scheduler.ScheduleAsync(async () => {
                    var tmp = await CrawlString($"{Env.ServerAddress}/api/status/{Env.Token}", new HttpClient());
                    Console.WriteLine($"{DateTime.Now.ToString()}: {tmp}");
                }).EveryThirtySeconds();
            });
            host.Run();
            //var service = new ServiceCollection();
            ////service.AddSingleton<>
            //using (var serviceProvider = service.BuildServiceProvider()) {
            //    var botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
            //    _ = serviceProvider.GetRequiredService<IOnCallbackQuery>();
            //    _ = serviceProvider.GetRequiredService<IOnMessage>();
            //    botClient.StartReceiving();
            //    Thread.Sleep(int.MaxValue);
            //}
        }
    }
}
