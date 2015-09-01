using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubServerCommon.Data
{
    public class RegisterSubServerData
    {
        public string GameServerAddress { get; set; }
        public Guid? ServerId { get; set; }
        public int? TcpPort { get; set; }
        public int? UdpPort { get; set; }
        public int ServerType { get; set; }
        public string ApplicationName { get; set; }
    }
}
