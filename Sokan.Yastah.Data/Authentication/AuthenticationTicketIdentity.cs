using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Authentication
{
    public class AuthenticationTicketIdentity
    {
        public AuthenticationTicketIdentity(
            long id,
            ulong userId)
        {
            Id = id;
            UserId = userId;
        }

        public long Id { get; }

        public ulong UserId { get; }

        internal static readonly Expression<Func<AuthenticationTicketEntity, AuthenticationTicketIdentity>> FromEntityProjection
            = e => new AuthenticationTicketIdentity(
                e.Id,
                e.UserId);
    }
}
