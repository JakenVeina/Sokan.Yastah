using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Data.Administration
{
    public interface IAdministrationActionRepository
    {
        Task<long> CreateAsync(
            int typeId,
            DateTimeOffset performed,
            ulong performedById,
            CancellationToken cancellationToken);
    }

    public class AdministrationActionRepository
        : IAdministrationActionRepository
    {
        public AdministrationActionRepository(
            YastahDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateAsync(
            int typeId,
            DateTimeOffset performed,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            var action = new AdministrationActionEntity()
            {
                TypeId = typeId,
                Performed = performed,
                PerformedById = performedById
            };

            await _context.AddAsync(action, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return action.Id;
        }

        private readonly YastahDbContext _context;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IAdministrationActionRepository, AdministrationActionRepository>();

    }
}
