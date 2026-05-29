

using documentvaultapi.DAL.Entities;

namespace documentvaultapi.DAL.Repositories.Interfaces
{
    public interface IApplicationMapRepository : IRepository<ApplicationMap>
    {
        Task<ApplicationMap?> ValidateAsync(long appId, Guid clientSecret);
        Task<ApplicationMap?> ValidateAsync(long appId, Guid clientSecret, string callerBaseUrl);
    }
}

