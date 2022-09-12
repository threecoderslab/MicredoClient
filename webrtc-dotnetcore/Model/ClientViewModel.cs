using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webrtc_dotnetcore.Model.Micred;

namespace webrtc_dotnetcore.Model
{
    public class ClientViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Organization> TopRatedOrganizations { get; set; }
        
    }
}
