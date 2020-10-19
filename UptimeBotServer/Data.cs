using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeBotServer {
    public class Data {
        public string NodeName { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsUp { get; set; }
    }
}
