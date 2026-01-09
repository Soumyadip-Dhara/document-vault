using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace documentvaultapi.DAL.Repositories
{
    public class ApplicationMapRepository :
        Repository<ApplicationMap, DocumentVaultDbContext>,
        IApplicationMapRepository
    {
        public ApplicationMapRepository(DocumentVaultDbContext context)
            : base(context) { }

        public async Task<ApplicationMap?> ValidateAsync(long appId, Guid clientSecret)
        {
            return await DocumentVaultDbContext.Set<ApplicationMap>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.AppId == appId &&
                    x.ClientSecret == clientSecret &&
                    x.IsActive == true);
        }

        public async Task<ApplicationMap?> ValidateAsync(
            long appId,
            Guid clientSecret,
            string callerBaseUrl)
        {
            var Res = await DocumentVaultDbContext.Set<ApplicationMap>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.AppId == appId &&
                    x.ClientSecret == clientSecret &&
                    x.BaseUrl == callerBaseUrl &&
                    x.IsActive == true);

            return Res;
        }
    }
}
