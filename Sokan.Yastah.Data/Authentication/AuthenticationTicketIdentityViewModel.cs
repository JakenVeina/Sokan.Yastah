using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Authentication
{
    public class AuthenticationTicketIdentityViewModel
    {
        public AuthenticationTicketIdentityViewModel(
            long id,
            ulong userId)
        {
            Id = id;
            UserId = userId;
        }

        public long Id { get; }

        public ulong UserId { get; }

        internal static readonly Expression<Func<AuthenticationTicketEntity, AuthenticationTicketIdentityViewModel>> FromEntityProjection
            = e => new AuthenticationTicketIdentityViewModel(
                e.Id,
                e.UserId);
    }
}
