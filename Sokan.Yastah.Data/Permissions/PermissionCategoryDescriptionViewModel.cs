using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Permissions
{
    public class PermissionCategoryDescriptionViewModel
    {
        public PermissionCategoryDescriptionViewModel(
            int id,
            string name,
            string description,
            IReadOnlyCollection<PermissionDescriptionViewModel> permissions)
        {
            Id = id;
            Name = name;
            Description = description;
            Permissions = permissions;
        }

        public int Id { get; }

        public string Name { get; }

        public string Description { get; }

        public IReadOnlyCollection<PermissionDescriptionViewModel> Permissions { get; }

        internal static readonly Expression<Func<PermissionCategoryEntity, PermissionCategoryDescriptionViewModel>> FromEntityProjection
            = e => new PermissionCategoryDescriptionViewModel(
                e.Id,
                e.Name,
                e.Description,
                e.Permissions
                    .Select(p => new PermissionDescriptionViewModel(
                        p.PermissionId,
                        p.Name,
                        p.Description))
                    .ToArray());
    }
}
