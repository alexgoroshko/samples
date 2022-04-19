using System.Collections.Generic;
using System.Threading.Tasks;
using UserServices.DTO;

namespace UserServices
{
    /// <summary>
    /// Define basic crud on users. 
    /// </summary>
    public interface IUserStorageProvider
    {
        public Task<User> GetUser(string id);
        public Task DeleteUser(string id);
        public Task<int> GetUserCount();
        public Task AcceptGdprTerms(string id, bool accept);
        public Task MoveToEnv(string id, string envName);
    }
    
    public interface IPersistentUserStorageProvider : IUserStorageProvider 
    {
        public Task<User> CreateUser(CreateUserDto createUserDto, string newTempPassword);
        public Task UpdateUser(UpdateUserDto user);
        public Task DisableUser(string id);
        public Task ResetPassword(string id, string newTempPassword);
        public Task<IEnumerable<User>> GetAllUsers();
    }

    public interface IVolatileUserStorageProvider : IUserStorageProvider 
    {
        public Task<User> CreateUser(User user);
        public Task UpdateUser(User user);
        public Task<IEnumerable<User>> GetAllUsers(bool includeDisabled = false);
    }


    public interface IUserService : IUserStorageProvider
    {
        public Task<IEnumerable<User>> GetAllUsers(bool includeDisabled = false);
        public Task<IEnumerable<User>> GetCompanyContactPersons(int companyId);
        public Task<IEnumerable<User>> GetAdminsForCompany(int companyId);
        public Task<User> CreateUser(CreateUserDto user);
        public Task UpdateUser(UpdateUserDto user);
        public Task DisableUser(string id);
        public Task ResetPassword(string id);
    }
}
