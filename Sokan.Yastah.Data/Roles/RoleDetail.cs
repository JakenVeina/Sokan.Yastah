using System;
using System.Linq.Expressions;

using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Roles
{
    public class RoleDetail
    {
        public long Id { get; internal set; }

        public long VersionId { get; internal set; }

        public string Name { get; internal set; }

        public bool IsDeleted { get; internal set; }

        public AdministrationActionBrief Creation { get; internal set; }

        public AdministrationActionBrief Update { get; internal set; }

        internal static Expression<Func<RoleVersionEntity, RoleVersionEntity, RoleDetail>> FromVersionEntitiesExpression
            => (firstVersion, currentVersion) => new RoleDetail()
            {
                Id = currentVersion.Role.Id,
                VersionId = currentVersion.Id,
                Name = currentVersion.Name,
                IsDeleted = currentVersion.IsDeleted,
                Creation = new AdministrationActionBrief()
                {
                    Performed = firstVersion.Action.Performed,
                    PerformedBy = firstVersion.Action.PerformedBy.Username
                },
                Update = new AdministrationActionBrief()
                {
                    Performed = currentVersion.Action.Performed,
                    PerformedBy = currentVersion.Action.PerformedBy.Username
                }
            };
    }
}
