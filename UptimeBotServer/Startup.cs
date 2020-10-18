using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Coravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace UptimeBotServer {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddScheduler();
            services.AddSingleton<ITelegramBotClient>(
                sp => string.IsNullOrEmpty(Env.HttpProxy) ? 
                new TelegramBotClient(Env.BotToken) : 
                new TelegramBotClient(Env.BotToken, new WebProxy(Env.HttpProxy)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            Global.Status = new Dictionary<string, Data>();
            var bot = app.ApplicationServices.GetRequiredService<ITelegramBotClient>();
            bot.StartReceiving();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
            app.ApplicationServices.UseScheduler(scheduler => {
                scheduler.ScheduleAsync(async () => {
                    var now = DateTime.UtcNow;
                    foreach (var e in Global.Status) {
                        if (e.Value.DateTime.AddMinutes(5) < now) {
                            var ErrorMessage = $"节点 {e.Key} 疑似离线超过5分钟";
                            Console.WriteLine(ErrorMessage);
                            await bot.SendTextMessageAsync(
                                chatId: Env.AdminId,
                                disableNotification: true,
                                parseMode: ParseMode.Markdown,
                                text: ErrorMessage
                                );
                        }
                    }
                }).EveryFiveMinutes();
            });
        }
    }
}
