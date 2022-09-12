using Microsoft.AspNetCore.Http;

namespace webrtc_dotnetcore.Model.Files
{
    public class FileDetails
    {
        public IFormFile FileContent { get; set; }
        public string User { get; set; }
    }
}