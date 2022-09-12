using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webrtc_dotnetcore.Model;
using webrtc_dotnetcore.Model.Micred;
using webrtc_dotnetcore.Services;

namespace webrtc_dotnetcore.Controllers
{
    public class MicredController : Controller
    {
        private readonly IMicred _micred;

        public MicredController(IMicred micred)
        {
            _micred = micred;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Agent()
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = Convert.ToInt32(claimsIdentity.FindFirst("UserId").Value);
            var userName = claimsIdentity.FindFirst("Username").Value;
            var organizationId = Convert.ToInt32(claimsIdentity.FindFirst("OrganizationId").Value);
            ViewBag.Userid = userId;
            ViewBag.UserName = userName;
            ViewBag.OrganizationId = organizationId;
            return View();
        }

        public IActionResult PreLogin()
        {
            return View();
        }

        public async Task<IActionResult> Client(int id,int roomId,string sessionId, int agentId) // call started
        {
            ViewBag.OrgId = id;
            ViewBag.RoomId = roomId;
            ViewBag.SessionId = sessionId;
            ViewBag.AgentId = agentId;
            var clientFields = await _micred.GetClientControlsByOrgId(id);
            return View(clientFields);
        }

        public IActionResult WebRTC()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var result = new ClientViewModel {
                Categories = _micred.GetCategories(),
                TopRatedOrganizations = await _micred.GetTopRatedOrganizations(),
            };
            return View(result);
        }
        [HttpGet]
        public IActionResult Organizations(int id)
        {
            List<Organization> organizations = _micred.GetOrganizations(id);
            return View(organizations);
        }

        [HttpPost]
        public JsonResult CreateNewActiveRoom(ActiveRoom activeRoom)
        {
            bool result = _micred.CreateActiveRoom(activeRoom);
            return Json("ok");
        }

        [HttpPost]
        public JsonResult DeleteActiveRoom(ActiveRoom activeRoom)
        {
            bool result = _micred.DeleteActiveRoom(activeRoom);
            return Json("ok");
        }

        [HttpGet]
        public JsonResult GetTopRatedOrganizations()
        {
            return Json(_micred.GetTopRatedOrganizations());
        }

        [HttpGet]
        public async Task<ViewResult> GetOrganizationDetails(int id)
        {
            var details = await _micred.GetOrganizationDetailsAsync(id);
            details.Id = id;
            var activeRoomData = await _micred.GetActiveRoomByOrgId(id);
            if (activeRoomData != null && activeRoomData.RoomId != 0)
            {
                details.IsHaveActiveAgent = true;
                details.RoomId = activeRoomData.RoomId;
                details.sessionid = activeRoomData.sessionid;
                details.RoomName = activeRoomData.RoomName;
                details.AgentId = activeRoomData.AgentId;
            }
            else
            {
                details.IsHaveActiveAgent = false;
            }
            return View("OrganizationWarningView", details);
        }

        [HttpGet]
        public async Task<JsonResult> CheckActiveOperators(int id)
        {
            var result = await _micred.GetActiveRoomByOrgId(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> StartCall(int agentId, string sessionId)
        {
            if (await _micred.StartCall(agentId, sessionId))
            {
                return Json("OK");
            }
            return Json("Call not inserted");
        }
    }
}
