using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MsGraph.Classes;
using MsGraph.GraphModels;
using UserServices.DTO;

namespace UserServices.Services.AzureUserServices
{
    public class AzureB2CUserStorageProvider : IPersistentUserStorageProvider
    {
        private readonly GraphClient _gc;
        private readonly IMapper _mapper;
        private readonly GraphConfig _graphConfig;
        private readonly string _userEnvName;

        public AzureB2CUserStorageProvider(GraphClient gc, IMapper mapper, GraphConfig graphConfig, string envName, string userEnvName)
        {
            _gc = gc;
            _mapper = mapper;
            _graphConfig = graphConfig;
            _userEnvName = userEnvName;
        }

        public async Task<User> GetUser(string id)
        {
            return _mapper.Map<User>(await _gc.GetUser(id));
        }
        
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            //only load users for the current environment
            var envPropertyFullName = _graphConfig.CustomAttributeFullName(nameof(User.EnvName));
            var filter = $"$filter={envPropertyFullName} eq '{_userEnvName}'";

            return _mapper.Map<IEnumerable<User>>((await _gc.GetUsers
                    (
                        999,
                        filterQuery: filter,
                        autoFollowNextLink: true)
                ).Items.Where(x => x.IsB2CUser)
            );
        }
        
        public async Task<User> CreateUser(CreateUserDto createUserDto, string newTempPassword)
        {
            var user = _mapper.Map<User>(createUserDto);
            user.Password = newTempPassword;
            var graphUser = _mapper.Map<GraphUser>(user);
            graphUser.ExtnEnvName = _userEnvName;
            return _mapper.Map<User>(await _gc.CreateUser(graphUser));
        }
        
        public async  Task UpdateUser(UpdateUserDto updateUserDto)
        {
            var user = _mapper.Map<User>(updateUserDto);
            var graphUser = _mapper.Map<GraphUser>(user);
            await _gc.UpdateUser(graphUser);
        }

        public async Task DisableUser(string id)
        {
            await _gc.DisableUser(id);
        }

        public async Task ResetPassword(string id, string newTempPassword)
        {
            await _gc.ResetPassword(id, newTempPassword);
        }
        
        public async Task AcceptGdprTerms(string id, bool accept)
        {
            var graphUser = new GraphUser
            {
                Id = id,
                ExtnGdprTermsAccepted = accept
            };
            await _gc.UpdateUser(graphUser);
        }

        public async Task MoveToEnv(string id, string envName)
        {
            var graphUser = new GraphUser
            {
                Id = id,
                ExtnEnvName = envName
            };
            await _gc.UpdateUser(graphUser);
        }
        
        public async Task DeleteUser(string id)
        {
            await _gc.DeleteUser(id);
        }

        public async Task<int> GetUserCount()
        {
            //unlikely to be used
            var allUsers = await GetAllUsers();
            return allUsers.ToList().Count;
        }
    }
}