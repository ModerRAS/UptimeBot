using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UptimeBotServer.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase {
        [HttpPost("{id}")]
        public IActionResult Post(Data data) {
            Global.Status.Add(data.NodeName, data);
            return Ok();
        }
    }
}