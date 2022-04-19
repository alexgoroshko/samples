using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace UserServices
{
    public class UserCacheInitialLoader : IHostedService
    {
        private readonly UserCache _userCache;

        public UserCacheInitialLoader(IVolatileUserStorageProvider volatileUserStorageProvider)
        {
            _userCache = (UserCache) volatileUserStorageProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _userCache.InitOrRefresh();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //this never gets called
            return Task.CompletedTask;
        }
    }
}