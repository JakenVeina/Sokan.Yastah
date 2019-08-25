using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Authentication
{
    public class AuthenticationTicketIdentity
    {
        public long Id { get; internal set; }

        public ulong UserId { get; internal set; }

        internal static readonly Expression<Func<AuthenticationTicketEntity, AuthenticationTicketIdentity>> FromEntityProjection
            = e => new AuthenticationTicketIdentity()
            {
                Id = e.Id,
                UserId = e.UserId
            };
    }
}
