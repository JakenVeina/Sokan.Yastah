using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Permissions
{
    public class PermissionIdentity
    {
        public int CategoryId { get; internal set; }

        public int PermissionId { get; internal set; }

        public string CategoryName { get; internal set; }

        public string PermissionName { get; internal set; }

        public string Name
            => $"{CategoryName}.{PermissionName}";

        internal static readonly Expression<Func<PermissionEntity, PermissionIdentity>> FromEntityProjection
            = entity => new PermissionIdentity()
            {
                CategoryId = entity.Category.Id,
                PermissionId = entity.PermissionId,
                CategoryName = entity.Category.Name,
                PermissionName = entity.Name
            };
    }
}
