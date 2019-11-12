using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Permissions
{
    public class PermissionIdentityViewModel
    {
        public int Id { get; internal set; }

        public string Name { get; internal set; }

        internal static readonly Expression<Func<PermissionEntity, PermissionIdentityViewModel>> FromEntityProjection
            = entity => new PermissionIdentityViewModel()
            {
                Id = entity.PermissionId,
                Name = entity.Category.Name + "." + entity.Name
            };
    }
}
