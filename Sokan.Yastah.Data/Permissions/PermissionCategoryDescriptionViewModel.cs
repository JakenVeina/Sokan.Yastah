using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Permissions
{
    public class PermissionCategoryDescriptionViewModel
    {
        public long Id { get; internal set; }

        public string Name { get; internal set; }

        public string Description { get; internal set; }

        public IReadOnlyCollection<PermissionDescriptionViewModel> Permissions { get; internal set; }

        internal static readonly Expression<Func<PermissionCategoryEntity, PermissionCategoryDescriptionViewModel>> FromEntityProjection
            = e => new PermissionCategoryDescriptionViewModel()
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Permissions = e.Permissions
                    .Select(p => new PermissionDescriptionViewModel()
                    {
                        Id = p.PermissionId,
                        Name = p.Name,
                        Description = p.Description
                    })
                    .ToArray()
            };
    }
}
