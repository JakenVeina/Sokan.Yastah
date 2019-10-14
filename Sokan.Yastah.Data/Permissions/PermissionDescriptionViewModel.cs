using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Permissions
{
    public class PermissionDescriptionViewModel
    {
        public int Id { get; internal set; }

        public string Name { get; internal set; }

        public string Description { get; internal set; }

        internal static readonly Expression<Func<PermissionEntity, PermissionDescriptionViewModel>> FromEntityProjection
            = e => new PermissionDescriptionViewModel()
            {
                Id = e.PermissionId,
                Name = e.Name,
                Description = e.Description
            };
    }
}
