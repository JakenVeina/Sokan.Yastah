using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Authentication
{
    public interface IAuthenticationTicketsRepository
    {
        IAsyncEnumerable<AuthenticationTicketIdentityViewModel> AsyncEnumerateIdentities(
            Optional<bool> isDeleted = default);

        Task<long> CreateAsync(
            ulong userId,
            long actionId,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteAsync(
            long ticketId,
            long actionId,
            CancellationToken cancellationToken);

        Task<OperationResult<long>> ReadActiveIdAsync(
            ulong userId,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class AuthenticationTicketsRepository
        : IAuthenticationTicketsRepository
    {
        public AuthenticationTicketsRepository(
            YastahDbContext context,
            ILogger<AuthenticationTicketsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IAsyncEnumerable<AuthenticationTicketIdentityViewModel> AsyncEnumerateIdentities(
            Optional<bool> isDeleted = default)
        {
            using var logScope = _logger.BeginMemberScope();
            AuthenticationLogMessages.IdentitiesEnumerating(_logger, isDeleted);

            var query = _context.Set<AuthenticationTicketEntity>()
                .AsQueryable();
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (isDeleted.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isDeleted));
                query = isDeleted.Value
                    ? query.Where(x => x.DeletionId != null)
                    : query.Where(x => x.DeletionId == null);
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query
                .Select(AuthenticationTicketIdentityViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public async Task<long> CreateAsync(
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            AuthenticationLogMessages.AuthenticationTicketCreating(_logger, userId, actionId);

            var ticket = new AuthenticationTicketEntity(
                id:         default,
                userId:     userId,
                creationId: actionId,
                deletionId: null);
            await _context.AddAsync(ticket, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            AuthenticationLogMessages.AuthenticationTicketCreated(_logger, ticket.Id);
            return ticket.Id;
        }

        public async Task<OperationResult> DeleteAsync(
            long ticketId,
            long actionId,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            AuthenticationLogMessages.AuthenticationTicketDeleting(_logger, ticketId, actionId);

            var ticket = await _context
                .FindAsync<AuthenticationTicketEntity?>(new object[] { ticketId }, cancellationToken);

            if (ticket is null)
            {
                AuthenticationLogMessages.AuthenticationTicketNotFound(_logger, ticketId);
                return new DataNotFoundError($"Authentication Ticket ID {ticketId}");
            }

            if (!(ticket.DeletionId is null))
            {
                AuthenticationLogMessages.AuthenticationTicketAlreadyDeleted(_logger, ticketId);
                return new DataAlreadyDeletedError($"Authentication Ticket ID {ticketId}");
            }

            ticket.DeletionId = actionId;

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            AuthenticationLogMessages.AuthenticationTicketDeleted(_logger, ticketId);
            return OperationResult.Success;
        }

        public async Task<OperationResult<long>> ReadActiveIdAsync(
                ulong userId,
                CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            AuthenticationLogMessages.ActiveIdReading(_logger, userId);

            var id = await _context.Set<AuthenticationTicketEntity>()
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .Where(x => x.DeletionId == null)
                .Select(x => (long?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (id is null)
            {
                AuthenticationLogMessages.AuthenticationTicketNotFoundForUser(_logger, userId);
                return new DataNotFoundError($"Active Authentication Ticket for User ID {userId}");
            }
            else
            {
                AuthenticationLogMessages.ActiveIdRead(_logger, userId, id.Value);
                return id.Value.ToSuccess();
            }
        }

        private readonly YastahDbContext _context;
        private readonly ILogger _logger;
    }
}
