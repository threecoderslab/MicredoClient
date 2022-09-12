using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using webrtc_dotnetcore.Model.Files;

namespace webrtc_dotnetcore.Controllers
{
    public class FilesController : Controller
    {
        private IWebHostEnvironment _hostingEnvironment;

        public FilesController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public JsonResult GetFile(FileDetails fileDetails)
        {
            try
            {
                var fileStream = new FileStream(_hostingEnvironment.WebRootPath + @"\files\" + fileDetails.FileContent.FileName, FileMode.Create);
                fileDetails.FileContent.CopyTo(fileStream);
                fileStream.Close();
                return Json("ok");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetCapture(FileDetails fileDetails)
        {
            try
            {
                var fileStream = new FileStream(_hostingEnvironment.WebRootPath + @"\screenshots\" + fileDetails.FileContent.FileName, FileMode.Create);
                fileDetails.FileContent.CopyTo(fileStream);
                fileStream.Close();
                return Json("ok");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
