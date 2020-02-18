using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Data.Administration
{
    public interface IAdministrationActionsRepository
    {
        Task<long> CreateAsync(
            int typeId,
            DateTimeOffset performed,
            ulong? performedById,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
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
            ulong? performedById,
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
    }
}
