using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Permissions
{
    public class PermissionIdentityViewModel
    {
        public PermissionIdentityViewModel(
            int id,
            string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }

        public string Name { get; }

        internal static readonly Expression<Func<PermissionEntity, PermissionIdentityViewModel>> FromEntityProjection
            = p => new PermissionIdentityViewModel(
                p.PermissionId,
                p.Category.Name + "." + p.Name);
    }
}
