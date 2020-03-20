using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            YastahDbContext context,
            ILogger<AdministrationActionsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<long> CreateAsync(
            int typeId,
            DateTimeOffset performed,
            ulong? performedById,
            CancellationToken cancellationToken)
        {
            AdministrationLogMessages.AdministrationActionCreating(_logger, typeId, performed, performedById);

            var action = new AdministrationActionEntity(
                id:             default,
                typeId:         typeId,
                performed:      performed,
                performedById:  performedById);
            await _context.AddAsync(action, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            AdministrationLogMessages.AdministrationActionCreated(_logger, action.Id);
            return action.Id;
        }

        private readonly YastahDbContext _context;
        private readonly ILogger _logger;
    }
}
