using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MsGraph.Classes;
using UserServices.DTO;
using UserServices.Enum;

namespace UserServices.Services.AzureUserServices
{
	public class AzureB2CUserService : IUserService
    {
        #region private

        private readonly GraphClient _gc;
        private readonly IPersistentUserStorageProvider _persistentUserStorageProvider;
        private readonly IVolatileUserStorageProvider _volatileUserStorageProvider;
        private readonly MailService.MailService _mailService;
        private readonly string _loginUrl;
        private readonly IMapper _mapper;
        
        private async Task SendInvitationEmail(User user, string tempPass)
        {
            var loginUrl = $"https://{_loginUrl}.azurewebsites.net/login";

            var msg = _mailService.ResetPasswordEmailTemplate(user.DisplayName, user.EmailLogin, tempPass, loginUrl);
            try { await _mailService.SendEmail(msg); }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
        }

        private async Task CheckLastAdmin(string id)
        {
            var user = await _volatileUserStorageProvider.GetUser(id);
            if (user.Role == nameof(UserRole.Admin))
            {
                var adminCount = (await _volatileUserStorageProvider.GetAllUsers()).Count(u => u.Role == nameof(UserRole.Admin));
                if (adminCount <= 1) throw new Exception($"Cannot delete user with id {id} because this is the last {nameof(UserRole.Admin)} in the system.");
            }
        }
        
        private bool HasReferences()
        {
            return false;
        }
        #endregion
        
        public AzureB2CUserService(
            GraphClient gc, 
            IPersistentUserStorageProvider persistentUserStorageProvider, 
            IVolatileUserStorageProvider volatileUserStorageProvider,
            MailService.MailService mailService,
            IMapper mapper,
            string loginUrl)
        {
            _gc = gc;
            _persistentUserStorageProvider = persistentUserStorageProvider;
            _volatileUserStorageProvider = volatileUserStorageProvider;
            _mailService = mailService;
            _loginUrl = loginUrl;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<User>> GetCompanyContactPersons(int companyId)
        {
            var users = await _volatileUserStorageProvider.GetAllUsers();
            return users.Where(u => u.CompanyId == companyId && u.IsCompanyContactPerson == true);
        }

        public async Task<IEnumerable<User>> GetAdminsForCompany(int companyId)
        {
            var users = await _volatileUserStorageProvider.GetAllUsers();
            return users.Where
            (
                u => u.CompanyId == companyId &&
                     u.Role is nameof(UserRole.CompanyAdmin) or nameof(UserRole.Admin) or nameof(UserRole.LocalCountryAdmin) &&
                     u.IsCompanyContactPerson == true
            );
        }

        public async Task<User> CreateUser(CreateUserDto createUserDto)
        {
            var pass = RandomDataGenerator.RandomPassword();
            //create user and get id from azure. Azure only returns basic record but we want all fields
            var createdUser = await _persistentUserStorageProvider.CreateUser(createUserDto, pass);
            //ask for all fields
            createdUser = await _persistentUserStorageProvider.GetUser(createdUser.Id);
			await SendInvitationEmail(createdUser, pass);
			//store in cache
			return await _volatileUserStorageProvider.CreateUser(createdUser);
        }

        public async Task UpdateUser(UpdateUserDto updateUserDto)
        {
            var id = updateUserDto.Id;
            await _persistentUserStorageProvider.UpdateUser(updateUserDto);
            //ask for all fields
            var updatedUser = await _persistentUserStorageProvider.GetUser(id);
            //store in cache
            await _volatileUserStorageProvider.UpdateUser(updatedUser);
        }

        public async Task ResetPassword(string id)
        {
            var user = await _volatileUserStorageProvider.GetUser(id);
            var pass = RandomDataGenerator.RandomPassword();
            await _persistentUserStorageProvider.ResetPassword(id, pass);

            //Azure will also set this time on its own so there may be a slight difference of a few seconds until the cache reloads but for our purpose it's okay
            user.LastPasswordChangeDateTime = DateTime.UtcNow;
            await SendInvitationEmail(user, pass);
        }

        public async Task AcceptGdprTerms(string id, bool accept)
        {
            await _persistentUserStorageProvider.AcceptGdprTerms(id, accept);
            await _volatileUserStorageProvider.AcceptGdprTerms(id, accept);
        }

        public async Task MoveToEnv(string id, string envName)
        {
            envName = envName.ToLower();
            await _persistentUserStorageProvider.MoveToEnv(id, envName);
            await _volatileUserStorageProvider.MoveToEnv(id, envName);
        }

        public Task<User> GetUser(string id)
        {
            return _volatileUserStorageProvider.GetUser(id);
        }

        public async Task<IEnumerable<User>> GetAllUsers(bool includeDisabled = false)
        {
            return await _volatileUserStorageProvider.GetAllUsers(includeDisabled);
        }
        
        public async Task DeleteUser(string id)
        {
            await CheckLastAdmin(id);
            if (!HasReferences())
            {
                await _persistentUserStorageProvider.DeleteUser(id);
                await _volatileUserStorageProvider.DeleteUser(id);
            }
            else
            {
                await DisableUser(id);
            }
        }

        public async Task DisableUser(string id)
        {
            await CheckLastAdmin(id);
            await _persistentUserStorageProvider.DisableUser(id);
            //get disabled user record from azure 
            var disabledUser = await _persistentUserStorageProvider.GetUser(id);
            //store in cache
            await _volatileUserStorageProvider.UpdateUser(disabledUser);
        }

        public async Task<int> GetUserCount()
        {
            return await _volatileUserStorageProvider.GetUserCount();
        }
    }
}
