using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using webrtc_dotnetcore.Model.Micred;
using Dapper;
using System.Data;
using Newtonsoft.Json;

namespace webrtc_dotnetcore.Services
{
    public class Micred : IMicred
    {
        private readonly IConfiguration _configuration;
        private readonly string _videochatConnection;
        private readonly string _createActiveRoom;
        private readonly string _deleteActiveRoom;
        private readonly string _getActiveRoomsByOrgId;
        private readonly string _getTopRatedOrgs;
        private readonly string _startcall;

        public Micred(IConfiguration configuration)
        {
            _configuration = configuration;
            _videochatConnection = _configuration.GetSection("ConnectionStrings")["videochat"];
            _createActiveRoom = _configuration.GetSection("DBProcedures")["createActiveRoom"];
            _deleteActiveRoom = _configuration.GetSection("DBProcedures")["deleteActiveRoom"];
            _getActiveRoomsByOrgId = _configuration.GetSection("DBProcedures")["getActiveRoomsByOrgId"];
            _getTopRatedOrgs = _configuration.GetSection("DBProcedures")["getTopRatedOrgs"];
            _startcall = _configuration.GetSection("DBProcedures")["startCall"];
        }

        public List<Category> GetCategories()
        {
            try
            {
                using var connection = new SqlConnection(_videochatConnection);
                var query = "SELECT id AS Id, [name] AS [Name], REPLACE(pict,'pic','img') AS Picture FROM sector";
                var categories = connection.Query<Category>(query, null, commandType: System.Data.CommandType.Text).ToList();
                return categories;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Organization> GetOrganizations(int id)
        {
            try
            {
                using var connection = new SqlConnection(_videochatConnection);
                var query = "SELECT [id] AS Id, [name] AS [Name], [logo] AS Logo  FROM [org] WHERE sectorid = " + id;
                var organizations = connection.Query<Organization>(query, null, commandType: System.Data.CommandType.Text).ToList();
                return organizations;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool CreateActiveRoom(ActiveRoom activeRoom)
        {
            try
            {
                using var connection = new SqlConnection(_videochatConnection);
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@RoomId", activeRoom.RoomId);
                parameters.Add("@RoomName", activeRoom.RoomName);
                parameters.Add("@AgentId", activeRoom.AgentId);
                var result = connection.Execute(_createActiveRoom, parameters, commandType: System.Data.CommandType.StoredProcedure);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool DeleteActiveRoom(ActiveRoom activeRoom)
        {
            try
            {
                using var connection = new SqlConnection(_videochatConnection);
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@RoomId", activeRoom.RoomId);
                parameters.Add("@RoomName", activeRoom.RoomName);
                parameters.Add("@AgentId", activeRoom.AgentId);
                var result = connection.Execute(_deleteActiveRoom, parameters, commandType: System.Data.CommandType.StoredProcedure);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ActiveRoom> GetActiveRoomsByOrgId(int organizationId)
        {
            try
            {
                using var connection = new SqlConnection(_videochatConnection);
                var parameters = new DynamicParameters();
                parameters.Add("@orgId", organizationId);
                return connection.Query<ActiveRoom>(_getActiveRoomsByOrgId, parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Organization>> GetTopRatedOrganizations()
        {
            try
            {
                using var connection = new SqlConnection(_videochatConnection);
                return connection.QueryAsync<Organization>($"SELECT * from {_getTopRatedOrgs}()", null, commandType: System.Data.CommandType.Text).Result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("500 error");
            }
        }

        public async Task<OrganizationDetails> GetOrganizationDetailsAsync(int orgId)
        {
            try
            {
                using var connection = new SqlConnection(_videochatConnection);
                string sql = @$"SELECT isnull([warningCl],'') warningCl,og.name,og.logo,og.sectorid
                                  FROM[dbo].[org_details] dt
                                  right outer join[dbo].[org] og on og.id = dt.[org_id]
                                  where og.id = {orgId}";
                var details = connection.QueryAsync<OrganizationDetails>(sql, null, null, null, System.Data.CommandType.Text).Result.FirstOrDefault();
                return details;
            }
            catch (Exception ex)
            {
                throw new Exception("500 error");
            }
        }

        public async Task<OrganizationDetails> GetActiveRoomByOrgId(int orgId)
        {
            try
            {
                using var connection = new SqlConnection(_videochatConnection);
                string sql = @$"Select {_getActiveRoomsByOrgId}({orgId})";
                var details = connection.QueryAsync<string>(sql, null, null, null, System.Data.CommandType.Text).Result.ToList();
                var roomDetalis = JsonConvert.DeserializeObject<RoomShortData>(details.FirstOrDefault());
                return new OrganizationDetails {
                    RoomId = roomDetalis.Roomid,
                    RoomName = roomDetalis.RoomName,
                    sessionid = roomDetalis.sessionId,
                    AgentId = roomDetalis.agentid
                };
            }
            catch (Exception ex)
            {
                throw new Exception("500 error");
            }
        }

        public async Task<List<ClientControl>> GetClientControlsByOrgId(int orgId)
        {
            try
            {
                using var connection = new SqlConnection(_videochatConnection);
                string sql = @$"select oc.control_id,input_type.type_name ,params.Item_ename,params.Item_name,params.length, params.pattern  from
                             [dbo].[Client_front_control] oc
                            join [dbo].[Client_control] params on oc.control_id=params.Item_id
                            join [dbo].[Client_control_types] input_type on input_type.id=params.type_id
                            where oc.org_id={orgId}";
                var controls = connection.QueryAsync<ClientControl>(sql, null, null, null, System.Data.CommandType.Text).Result.ToList();
                return controls;
            }
            catch (Exception ex)
            {
                throw new Exception("500 error");
            }
        }

        public async Task<bool> StartCall(int agentId, string sessionId)
        {
            try
            {
                using var connection = new SqlConnection(_videochatConnection);
                var parameters = new DynamicParameters();
                parameters.Add("@sessionid", sessionId);
                parameters.Add("@agid", agentId);
                var result = await connection.ExecuteAsync(_startcall, parameters, null, null, CommandType.StoredProcedure);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("500 error");
            }
        }
    }
}
