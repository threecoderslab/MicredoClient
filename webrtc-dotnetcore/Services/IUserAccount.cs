using System.Threading.Tasks;
using webrtc_dotnetcore.Model.Account;

namespace webrtc_dotnetcore.Services
{
    public interface IUserAccount
    {
        Task<User> Login(User user);
    }
}