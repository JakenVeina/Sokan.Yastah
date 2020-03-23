using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data.Auditing
{
    public interface IAuditableActionsRepository
    {
        Task<long> CreateAsync(
            int typeId,
            DateTimeOffset performed,
            ulong? performedById,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class AuditableActionsRepository
        : IAuditableActionsRepository
    {
        public AuditableActionsRepository(
            YastahDbContext context,
            ILogger<AuditableActionsRepository> logger)
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
            AuditingLogMessages.AuditableActionCreating(_logger, typeId, performed, performedById);

            var action = new AuditableActionEntity(
                id:             default,
                typeId:         typeId,
                performed:      performed,
                performedById:  performedById);
            await _context.AddAsync(action, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            AuditingLogMessages.AuditableActionCreated(_logger, action.Id);
            return action.Id;
        }

        private readonly YastahDbContext _context;
        private readonly ILogger _logger;
    }
}
