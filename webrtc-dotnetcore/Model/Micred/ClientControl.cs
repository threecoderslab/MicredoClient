using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webrtc_dotnetcore.Model.Micred
{
    public class ClientControl
    {
        public int control_id { get; set; }
        public string item_name { get; set; }
        public string item_ename { get; set; }
        public string type_name { get; set; }
        public int length { get; set; }
        public string pattern { get; set; }
    }
}
