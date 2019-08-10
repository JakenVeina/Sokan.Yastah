using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Roles
{
    public class RoleIdentityViewModel
    {
        public long Id { get; internal set; }

        public string Name { get; internal set; }

        internal static readonly Expression<Func<RoleVersionEntity, RoleIdentityViewModel>> FromVersionEntityProjection
            = ve => new RoleIdentityViewModel()
            {
                Id = ve.RoleId,
                Name = ve.Name
            };
    }
}
