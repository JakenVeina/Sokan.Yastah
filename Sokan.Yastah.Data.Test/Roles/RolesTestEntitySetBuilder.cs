using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data.Test.Roles
{
    internal class RolesTestEntitySetBuilder
        : YastahTestEntitySetBuilder
    {
        public static readonly RolesTestEntitySetBuilder Instance
            = new RolesTestEntitySetBuilder();

        public static readonly YastahTestEntitySet SharedSet
            = NewSet();

        public static YastahTestEntitySet NewSet()
            => Instance.Build();

        private RolesTestEntitySetBuilder() { }

        protected override IReadOnlyList<RoleEntity>? CreateRoles()
            => Enumerable.Empty<RoleEntity>()
                .Append(new RoleEntity( id: 1   ))
                .Append(new RoleEntity( id: 2   ))
                .Append(new RoleEntity( id: 3   ))
                .ToArray();

        protected override IReadOnlyList<RolePermissionMappingEntity>? CreateRolePermissionMappings()
            => Enumerable.Empty<RolePermissionMappingEntity>()
                .Append(new RolePermissionMappingEntity(    id: 1,  roleId: 1,  permissionId: 1,    creationId: 4,  deletionId: null    ))
                .Append(new RolePermissionMappingEntity(    id: 2,  roleId: 2,  permissionId: 2,    creationId: 6,  deletionId: 8       ))
                .Append(new RolePermissionMappingEntity(    id: 3,  roleId: 2,  permissionId: 3,    creationId: 8,  deletionId: null    ))
                .Append(new RolePermissionMappingEntity(    id: 4,  roleId: 3,  permissionId: 1,    creationId: 11, deletionId: null    ))
                .Append(new RolePermissionMappingEntity(    id: 5,  roleId: 3,  permissionId: 2,    creationId: 11, deletionId: 13      ))
                .Append(new RolePermissionMappingEntity(    id: 6,  roleId: 3,  permissionId: 3,    creationId: 11, deletionId: null    ))
                .ToArray();

        protected override IReadOnlyList<RoleVersionEntity>? CreateRoleVersions()
            => Enumerable.Empty<RoleVersionEntity>()
                .Append(new RoleVersionEntity(  id: 1,  roleId: 1,  name: "Role 1",     isDeleted: false,   creationId: 2,    nextVersionId: null,    previousVersionId: null ))
                .Append(new RoleVersionEntity(  id: 2,  roleId: 2,  name: "Role 2",     isDeleted: false,   creationId: 10,   nextVersionId: 3,       previousVersionId: null ))
                .Append(new RoleVersionEntity(  id: 3,  roleId: 2,  name: "Role 2a",    isDeleted: false,   creationId: 12,   nextVersionId: 5,       previousVersionId: 2    ))
                .Append(new RoleVersionEntity(  id: 4,  roleId: 3,  name: "Role 3",     isDeleted: false,   creationId: 14,   nextVersionId: 6,       previousVersionId: null ))
                .Append(new RoleVersionEntity(  id: 5,  roleId: 2,  name: "Role 2a",    isDeleted: true,    creationId: 15,   nextVersionId: null,    previousVersionId: 3    ))
                .Append(new RoleVersionEntity(  id: 6,  roleId: 3,  name: "Role 3a",    isDeleted: true,    creationId: 16,   nextVersionId: 7,       previousVersionId: 4    ))
                .Append(new RoleVersionEntity(  id: 7,  roleId: 3,  name: "Role 3",     isDeleted: false,   creationId: 17,   nextVersionId: null,    previousVersionId: 7    ))
                .ToArray();
    }
}
