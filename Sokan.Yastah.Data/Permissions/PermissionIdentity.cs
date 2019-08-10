using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Permissions
{
    public class PermissionIdentity
    {
        public int Id { get; internal set; }

        public string Name { get; internal set; }

        internal static readonly Expression<Func<PermissionEntity, PermissionIdentity>> FromEntityProjection
            = entity => new PermissionIdentity()
            {
                Id = entity.PermissionId,
                Name = entity.Category.Name + "." + entity.Name
            };
    }
}
