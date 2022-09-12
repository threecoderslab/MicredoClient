using System.ComponentModel.DataAnnotations;

namespace webrtc_dotnetcore.Model.Account
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public int OrganizationId { get; set; }
        public string RoomName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
