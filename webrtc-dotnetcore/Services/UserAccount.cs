using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using webrtc_dotnetcore.Model.Account;
using Dapper;
using System.Data;

namespace webrtc_dotnetcore.Services
{
    public class UserAccount : IUserAccount
    {
        private string _connectionString;
        private string _getUserDetails;

        public UserAccount(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings")["videochat"];
            _getUserDetails = configuration.GetSection("DBProcedures")["getUserDetails"];
        }

        public async Task<User> Login(User user)
        {
            try
            {
                User userDetails = new User();
                using SqlConnection connection = new SqlConnection(_connectionString);
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@username", user.UserName);
                parameters.Add("@password", user.Password);
                userDetails = await connection.QueryFirstAsync<User>(_getUserDetails, parameters, commandType: CommandType.StoredProcedure);
                return userDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
