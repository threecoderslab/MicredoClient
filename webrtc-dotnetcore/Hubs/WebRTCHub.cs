using Dapper;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using webrtc_dotnetcore.Model;
using webrtc_dotnetcore.Model.Micred;

namespace webrtc_dotnetcore.Hubs
{
    public class WebRTCHub : Hub
    {
        private static readonly RoomManager roomManager = new RoomManager();

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            roomManager.DeleteRoom(Context.ConnectionId);
            _ = NotifyRoomInfoAsync(false, 0);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task CreateRoom(string name)
        {
            RoomInfo roomInfo = roomManager.CreateRoom(Context.ConnectionId, name);
            if (roomInfo != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomInfo.RoomId);
                await Clients.Caller.SendAsync("created", roomInfo.RoomId);
                await NotifyRoomInfoAsync(false, 0);
            }
            else
            {
                await Clients.Caller.SendAsync("error", "error occurred when creating a new room.");
            }
        }

        public async Task Join(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Caller.SendAsync("joined", roomId);
            await Clients.Group(roomId).SendAsync("ready");

            //remove the room from room list.
            if (int.TryParse(roomId, out int id))
            {
                roomManager.DeleteRoom(id);
                await NotifyRoomInfoAsync(false, 0);
            }
        }

        public async Task LeaveRoom(string roomId)
        {
            await Clients.Group(roomId).SendAsync("bye");
            roomManager.DeleteRoom(roomId);
            await NotifyRoomInfoAsync(false, 0);
        }

        public async Task GetRoomInfo(int orgId)
        {
            await NotifyRoomInfoAsync(true, orgId);
        }

        public async Task SendMessage(string roomId, object message)
        {
            await Clients.OthersInGroup(roomId).SendAsync("message", message);
        }

        public async Task NotifyRoomInfoAsync(bool notifyOnlyCaller, int orgId)
        {

            List<RoomInfo> roomInfos = roomManager.GetAllRoomInfo();
            IEnumerable<string> allRoomIds = roomInfos.Select(a => a.RoomId);

            var connectionString = "Data Source= master.am.mssql;Initial Catalog=videochat; Persist Security Info=True;User ID=restuser;Password=Acode2017!bedo";
            using var connection = new SqlConnection(connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@orgId", orgId);
            IEnumerable<ActiveRoom> activeRooms = connection.Query<ActiveRoom>("[dbo].[GetActiveRoomsByOrgId]", parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
            IEnumerable<string> activeRoomids = activeRooms.Select(a => a.RoomId.ToString());


            IEnumerable<string> duplicates = allRoomIds.Intersect(activeRoomids);

            if (activeRooms.Any())
            {
                foreach (var roomId in duplicates)
                {
                    //var roomInfos1 = roomInfos.Where(rId => rId.RoomId.Contains(roomId));
                    roomInfos.RemoveAll(room => room.RoomId != roomId);
                }
            }
            else
            {
                roomInfos = null;
            }



            var list = from room in roomInfos
                       select new {
                           RoomId = room.RoomId,
                           Name = room.Name,
                           Button = "<button class=\"joinButton\">Join!</button>"
                       };
            var data = JsonConvert.SerializeObject(list);

            if (notifyOnlyCaller)
            {
                await Clients.Caller.SendAsync("updateRoom", data);
            }
            else
            {
                await Clients.All.SendAsync("updateRoom", data);
            }
        }
    }
}
