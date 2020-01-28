using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Authentication;

namespace Sokan.Yastah.Data.Test.Authentication
{
    internal class AuthenticationTicketsTestEntitySetBuilder
        : YastahTestEntitySetBuilder
    {
        private static readonly AuthenticationTicketsTestEntitySetBuilder Instance
            = new AuthenticationTicketsTestEntitySetBuilder();

        public static readonly YastahTestEntitySet SharedSet
            = NewSet();

        public static YastahTestEntitySet NewSet()
            => Instance.Build();

        private AuthenticationTicketsTestEntitySetBuilder() { }

        protected override IReadOnlyList<AuthenticationTicketEntity>? CreateAuthenticationTickets()
            => Enumerable.Empty<AuthenticationTicketEntity>()
                .Append(new AuthenticationTicketEntity( id: 1,  userId: 1,  creationId: 1,  deletionId: 5))
                .Append(new AuthenticationTicketEntity( id: 2,  userId: 2,  creationId: 3,  deletionId: 7))
                .Append(new AuthenticationTicketEntity( id: 3,  userId: 1,  creationId: 5,  deletionId: null))
                .Append(new AuthenticationTicketEntity( id: 4,  userId: 3,  creationId: 7,  deletionId: null))
                .Append(new AuthenticationTicketEntity( id: 5,  userId: 2,  creationId: 9,  deletionId: null))
                .ToArray();
    }
}
