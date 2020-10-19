using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace UptimeBotServer.Services {
    public class BotService {
        private readonly ITelegramBotClient botClient;
        public BotService(ITelegramBotClient botClient) {
            this.botClient = botClient;
        }

        private async Task SendMessage(string message) {
            await botClient.SendTextMessageAsync(
                                chatId: Env.AdminId,
                                disableNotification: true,
                                parseMode: ParseMode.Markdown,
                                text: message
                                );
        }
        public async Task DeviceUp(string device) {
            var message = $"设备 {device} 上线";
            await SendMessage(message);
        }
        public async Task DeviceDown(string device) {
            var message = $"设备 {device} 超过5分钟未回报， 疑似掉线";
            await SendMessage(message);
        }
    }
}
