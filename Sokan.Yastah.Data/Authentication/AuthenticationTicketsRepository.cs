using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

    public class AuthenticationTicketsRepository
        : IAuthenticationTicketsRepository
    {
        public AuthenticationTicketsRepository(
            YastahDbContext context)
        {
            _context = context;
        }

        public IAsyncEnumerable<AuthenticationTicketIdentityViewModel> AsyncEnumerateIdentities(
            Optional<bool> isDeleted = default)
        {
            var query = _context.Set<AuthenticationTicketEntity>()
                .AsQueryable();

            if (isDeleted.IsSpecified)
                query = isDeleted.Value
                    ? query.Where(x => x.DeletionId != null)
                    : query.Where(x => x.DeletionId == null);

            return query
                .Select(AuthenticationTicketIdentityViewModel.FromEntityProjection)
                .AsAsyncEnumerable();
        }

        public async Task<long> CreateAsync(
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var ticket = new AuthenticationTicketEntity(
                id:         default,
                userId:     userId,
                creationId: actionId,
                deletionId: null);

            await _context.AddAsync(ticket, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return ticket.Id;
        }

        public async Task<OperationResult> DeleteAsync(
            long ticketId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var ticket = await _context
                .FindAsync<AuthenticationTicketEntity?>(new object[] { ticketId }, cancellationToken);

            if (ticket is null)
                return new DataNotFoundError($"Authentication Ticket ID {ticketId}")
                    .ToError();

            if (!(ticket.DeletionId is null))
                return new DataAlreadyDeletedError($"Authentication Ticket ID {ticketId}")
                    .ToError();

            ticket.DeletionId = actionId;

            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult.Success;
        }

        public async Task<OperationResult<long>> ReadActiveIdAsync(
                ulong userId,
                CancellationToken cancellationToken)
        {
            var id = await _context.Set<AuthenticationTicketEntity>()
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .Where(x => x.DeletionId == null)
                .Select(x => (long?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return (id is null)
                ? new DataNotFoundError($"Active Authentication Ticket for User ID {userId}").ToError<long>()
                : id.Value.ToSuccess();
        }

        private readonly YastahDbContext _context;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services.AddScoped<IAuthenticationTicketsRepository, AuthenticationTicketsRepository>();
    }
}
