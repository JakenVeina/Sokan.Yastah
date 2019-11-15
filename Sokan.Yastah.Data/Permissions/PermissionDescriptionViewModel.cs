using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Permissions
{
    public class PermissionDescriptionViewModel
    {
        public PermissionDescriptionViewModel(
            int id,
            string name,
            string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public int Id { get; }

        public string Name { get; }

        public string Description { get; }

        internal static readonly Expression<Func<PermissionEntity, PermissionDescriptionViewModel>> FromEntityProjection
            = e => new PermissionDescriptionViewModel(
                e.PermissionId,
                e.Name,
                e.Description);
    }
}
