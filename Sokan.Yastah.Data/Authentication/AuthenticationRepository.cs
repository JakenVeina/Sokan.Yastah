using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Authentication
{
    public interface IAuthenticationRepository
    {
        Task<long> CreateTicketAsync(
            ulong userId,
            long actionId,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteTicketAsync(
            long ticketId,
            long actionId,
            CancellationToken cancellationToken);

        Task<OperationResult<long>> ReadActiveTicketId(
            ulong userId,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<AuthenticationTicketIdentity>> ReadTicketIdentities(
            CancellationToken cancellationToken,
            Optional<bool> isDeleted = default);
    }

    public class AuthenticationRepository
        : IAuthenticationRepository
    {
        public AuthenticationRepository(
            YastahDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateTicketAsync(
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var ticket = new AuthenticationTicketEntity()
            {
                UserId = userId,
                CreationId = actionId
            };

            await _context.AddAsync(ticket, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return ticket.Id;
        }

        public async Task<OperationResult> DeleteTicketAsync(
            long ticketId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var ticket = await _context.Set<AuthenticationTicketEntity>()
                .FindAsync(new object[] { ticketId }, cancellationToken);

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

        public async Task<OperationResult<long>> ReadActiveTicketId(
                ulong userId,
                CancellationToken cancellationToken)
        {
            var id = await _context.Set<AuthenticationTicketEntity>()
                .Where(x => x.UserId == userId)
                .Where(x => x.DeletionId == null)
                .Select(x => (long?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return (id is null)
                ? new DataNotFoundError($"Active Authentication Ticket for User ID {userId}").ToError<long>()
                : id.Value.ToSuccess();
        }

        public async Task<IReadOnlyCollection<AuthenticationTicketIdentity>> ReadTicketIdentities(
            CancellationToken cancellationToken,
            Optional<bool> isDeleted = default)
        {
            var query = _context.Set<AuthenticationTicketEntity>()
                .AsQueryable();

            if (isDeleted.IsSpecified)
                query = isDeleted.Value
                    ? query.Where(x => x.DeletionId != null)
                    : query.Where(x => x.DeletionId == null);

            return await query
                .Select(AuthenticationTicketIdentity.FromEntityProjection)
                .ToArrayAsync(cancellationToken);
        }

        private readonly YastahDbContext _context;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
    }
}
