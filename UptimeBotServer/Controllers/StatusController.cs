using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UptimeBotServer.Services;

namespace UptimeBotServer.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase {
        private readonly BotService botService;
        public StatusController(BotService botService) {
            this.botService = botService;
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(Data data) {
            Data value;
            if (Global.Status.TryGetValue(data.NodeName, out value)) {
                if (!value.IsUp) {
                    await botService.DeviceUp(data.NodeName);
                }
            } else {
                await botService.DeviceUp(data.NodeName);
            }
            Global.Status.Add(data.NodeName, data);
            return Ok();
        }
    }
}