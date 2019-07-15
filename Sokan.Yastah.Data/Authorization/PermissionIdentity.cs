using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Authorization
{
    public class PermissionIdentity
    {
        public long CategoryId { get; internal set; }

        public long PermissionId { get; internal set; }

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
