using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sokan.Yastah.Data.Administration
{
    public interface IAdministrationActionsRepository
    {
        Task<long> CreateAsync(
            int typeId,
            DateTimeOffset performed,
            ulong performedById,
            CancellationToken cancellationToken);
    }

    public class AdministrationActionsRepository
        : IAdministrationActionsRepository
    {
        public AdministrationActionsRepository(
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
            var action = new AdministrationActionEntity(
                id:             default,
                typeId:         typeId,
                performed:      performed,
                performedById:  performedById);

            await _context.AddAsync(action, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return action.Id;
        }

        private readonly YastahDbContext _context;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IAdministrationActionsRepository, AdministrationActionsRepository>();

    }
}
