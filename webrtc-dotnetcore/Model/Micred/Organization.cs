namespace webrtc_dotnetcore.Model.Micred
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
    }

    public class OrganizationDetails : Organization
    {
        public string warningCl { get; set; }
        public bool IsHaveActiveAgent { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string sessionid { get; set; }
        public int AgentId { get; set; }
    }

    public class RoomShortData
    {
        public int Roomid { get; set; }
        public string RoomName { get; set; }
        public string sessionId { get; set; }
        public int agentid { get; set; }
    }
}