using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Roles
{
    public class RoleIdentityViewModel
    {
        public RoleIdentityViewModel(
            long id,
            string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }

        public string Name { get; }

        internal static readonly Expression<Func<RoleVersionEntity, RoleIdentityViewModel>> FromVersionEntityProjection
            = ve => new RoleIdentityViewModel(
                ve.RoleId,
                ve.Name);
    }
}
