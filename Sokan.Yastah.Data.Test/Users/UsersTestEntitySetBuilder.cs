using System;
using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Roles;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test.Users
{
    internal class UsersTestEntitySetBuilder
        : YastahTestEntitySetBuilder
    {
        public static readonly YastahTestEntitySet SharedSet
            = new UsersTestEntitySetBuilder().Build();

        private UsersTestEntitySetBuilder() { }

        protected override IReadOnlyList<DefaultPermissionMappingEntity>? CreateDefaultPermissionMappings()
            => Enumerable.Empty<DefaultPermissionMappingEntity>()
                .Append(new DefaultPermissionMappingEntity( id: 1,  permissionId: 1,    creationId: 18, deletionId: null    ))
                .Append(new DefaultPermissionMappingEntity( id: 2,  permissionId: 2,    creationId: 20, deletionId: 21      ))
                .Append(new DefaultPermissionMappingEntity( id: 3,  permissionId: 3,    creationId: 21, deletionId: 22      ))
                .Append(new DefaultPermissionMappingEntity( id: 4,  permissionId: 2,    creationId: 23, deletionId: null    ))
                .ToArray();

        protected override IReadOnlyList<DefaultRoleMappingEntity>? CreateDefaultRoleMappings()
            => Enumerable.Empty<DefaultRoleMappingEntity>()
                .Append(new DefaultRoleMappingEntity(   id: 1,  roleId: 1,  creationId: 19, deletionId: null    ))
                .Append(new DefaultRoleMappingEntity(   id: 2,  roleId: 2,  creationId: 21, deletionId: 24      ))
                .Append(new DefaultRoleMappingEntity(   id: 3,  roleId: 3,  creationId: 24, deletionId: 26      ))
                .Append(new DefaultRoleMappingEntity(   id: 4,  roleId: 2,  creationId: 25, deletionId: null    ))
                .ToArray();

        protected override IReadOnlyList<PermissionEntity>? CreatePermissions()
            => Enumerable.Empty<PermissionEntity>()
                .Append(new PermissionEntity(   permissionId: 1,    categoryId: 1,  name: "Permission 1, Category 1",   description: "Permission #1, within Permissions Category #1"    ))
                .Append(new PermissionEntity(   permissionId: 2,    categoryId: 2,  name: "Permission 1, Category 2",   description: "Permission #1, within Permissions Category #2"    ))
                .Append(new PermissionEntity(   permissionId: 3,    categoryId: 2,  name: "Permission 2, Category 2",   description: "Permission #2, within Permissions Category #2"    ))
                .ToArray();

        protected override IReadOnlyList<PermissionCategoryEntity>? CreatePermissionCategories()
            => Enumerable.Empty<PermissionCategoryEntity>()
                .Append(new PermissionCategoryEntity(id: 1, name: "Category 1", description: "Permissions Category #1"))
                .Append(new PermissionCategoryEntity(id: 2, name: "Category 2", description: "Permissions Category #2"))
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

        protected override IReadOnlyList<UserEntity>? CreateUsers()
            => Enumerable.Empty<UserEntity>()
                .Append(new UserEntity( id: 1,  username: "User 1", discriminator: "0001",  avatarHash: "00001",    firstSeen: DateTimeOffset.Parse("2019-01-01"),  lastSeen: DateTimeOffset.Parse("2019-01-05")    ))
                .Append(new UserEntity( id: 2,  username: "User 2", discriminator: "0002",  avatarHash: "00002",    firstSeen: DateTimeOffset.Parse("2019-01-03"),  lastSeen: DateTimeOffset.Parse("2019-01-07")    ))
                .Append(new UserEntity( id: 3,  username: "User 3", discriminator: "0003",  avatarHash: "00003",    firstSeen: DateTimeOffset.Parse("2019-01-09"),  lastSeen: DateTimeOffset.Parse("2019-01-09")    ))
                .ToArray();

        protected override IReadOnlyList<UserPermissionMappingEntity>? CreateUserPermissionMappings()
            => Enumerable.Empty<UserPermissionMappingEntity>()
                .Append(new UserPermissionMappingEntity(    id: 1,  userId: 1,  permissionId: 3,    isDenied: false,    creationId: 27, deletionId: 31      ))
                .Append(new UserPermissionMappingEntity(    id: 2,  userId: 3,  permissionId: 1,    isDenied: true,     creationId: 28, deletionId: null    ))
                .Append(new UserPermissionMappingEntity(    id: 3,  userId: 3,  permissionId: 2,    isDenied: true,     creationId: 28, deletionId: null    ))
                .Append(new UserPermissionMappingEntity(    id: 4,  userId: 2,  permissionId: 3,    isDenied: false,    creationId: 29, deletionId: null    ))
                .Append(new UserPermissionMappingEntity(    id: 5,  userId: 2,  permissionId: 2,    isDenied: false,    creationId: 29, deletionId: 32      ))
                .Append(new UserPermissionMappingEntity(    id: 6,  userId: 2,  permissionId: 1,    isDenied: true,     creationId: 30, deletionId: null    ))
                .Append(new UserPermissionMappingEntity(    id: 7,  userId: 1,  permissionId: 2,    isDenied: false,    creationId: 31, deletionId: null    ))
                .Append(new UserPermissionMappingEntity(    id: 8,  userId: 1,  permissionId: 1,    isDenied: false,    creationId: 31, deletionId: null    ))
                .ToArray();

        protected override IReadOnlyList<UserRoleMappingEntity>? CreateUserRoleMappings()
            => Enumerable.Empty<UserRoleMappingEntity>()
                .Append(new UserRoleMappingEntity(  id: 1,  userId: 3,  roleId: 1,  creationId: 33, deletionId: 38      ))
                .Append(new UserRoleMappingEntity(  id: 2,  userId: 3,  roleId: 3,  creationId: 34, deletionId: 38      ))
                .Append(new UserRoleMappingEntity(  id: 3,  userId: 2,  roleId: 3,  creationId: 35, deletionId: null    ))
                .Append(new UserRoleMappingEntity(  id: 4,  userId: 2,  roleId: 1,  creationId: 35, deletionId: null    ))
                .Append(new UserRoleMappingEntity(  id: 5,  userId: 1,  roleId: 2,  creationId: 36, deletionId: null    ))
                .Append(new UserRoleMappingEntity(  id: 6,  userId: 1,  roleId: 1,  creationId: 37, deletionId: 39      ))
                .ToArray();
    }
}
