using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webrtc_dotnetcore.Model.Micred;

namespace webrtc_dotnetcore.Services
{
    public interface IMicred
    {
        List<Category> GetCategories();
        List<Organization> GetOrganizations(int id);
        bool CreateActiveRoom(ActiveRoom activeRoom);
        bool DeleteActiveRoom(ActiveRoom activeRoom);
        List<ActiveRoom> GetActiveRoomsByOrgId(int organizationId);
        Task<List<Organization>> GetTopRatedOrganizations();
        Task<OrganizationDetails> GetOrganizationDetailsAsync(int orgId);
        Task<OrganizationDetails> GetActiveRoomByOrgId(int orgId);
        Task<List<ClientControl>> GetClientControlsByOrgId(int orgId);
        Task<bool> StartCall(int agentId, string sessionId);
    }
}
