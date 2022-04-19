using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MsGraph.Classes;

namespace UserServices
{
    public class UserCache :IVolatileUserStorageProvider
    {
        #region private
        private readonly IPersistentUserStorageProvider _persistentUserStorageProvider;

        private List<User> _users;

        private void CheckReady()
        {
            if (!Ready) throw new Exception("Cache not ready"); 
        }

        /// <summary>
        /// Indicates that all data has been loaded
        /// </summary>
        private bool Ready { get; set; }

        private List<User> Users
        {
            get
            {
                CheckReady();
                return _users;
            }
            set => _users = value;
        }

        private void CacheError(int statusCode, string message)
        {
            throw new GraphStatusCodeException(statusCode, "user cache error: " + message);
        }

        private User FindUser(string id)
        {
            var user= Users.FirstOrDefault(x => x.Id == id);
            if (user == null) CacheError(404, $"user not found (id: {id})");
            return user;
        }
        #endregion private


        #region IUserStorageProvider
        public Task<User> GetUser(string id)
        {
            return Task.FromResult(FindUser(id));
        }

        public Task<IEnumerable<User>> GetAllUsers(bool includeDisabled = false)
        {
            var result = includeDisabled ? Users : Users.Where(u => u.AccountEnabled == true);
            return Task.FromResult(result);
        }
        
        public Task DeleteUser(string id)
        {
            var user = FindUser(id);
            Users.Remove(user);
            return Task.CompletedTask;
        }

        public Task<int> GetUserCount()
        {
            return Task.FromResult(Users.Count);
        }

        public Task AcceptGdprTerms(string id, bool accept)
        {
            var user = FindUser(id);
            user.GdprTermsAccepted = accept;
            return Task.CompletedTask;
        }

        public Task MoveToEnv(string id, string envName)
        {
            var user = FindUser(id);
            user.EnvName = envName;
            return Task.CompletedTask;
        }
        #endregion IUserStorageProvider

        #region IVolatileUserStorageProvider
        public Task<User> CreateUser(User user)
        {
            if (string.IsNullOrEmpty(user.Id)) CacheError(400, "missing ID");
            if (Users.Any(x => x.Id == user.Id)) CacheError(400, "duplicate ID");
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task UpdateUser(User user)
        {
            var existingUser = FindUser(user.Id);
            //if (existingUser == null) CacheError(404, "could not find user to update");
            Users.Remove(existingUser);
            Users.Add(user);
            return Task.CompletedTask;
        }
        #endregion IVolatileUserStorageProvider

        #region Public
        public UserCache(IPersistentUserStorageProvider persistentUserStorageProvider)
        {
            _persistentUserStorageProvider = persistentUserStorageProvider;
        }
        
        public async Task InitOrRefresh()
        {
            var freshUsers = await _persistentUserStorageProvider.GetAllUsers();
            _users = freshUsers.ToList();
            Ready = true;
        }
        #endregion Public
    }
}
