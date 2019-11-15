using System;
using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Authentication;
using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Roles;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test
{
    internal class YastahTestEntitySet
    {
        public static readonly YastahTestEntitySet Default
            = new YastahTestEntitySet();

        public YastahTestEntitySet()
        {
            PermissionCategories = Enumerable.Empty<PermissionCategoryEntity>()
                .Append(new PermissionCategoryEntity(   id: 1,  name: "Administration", description: "Permissions related to administration of the application" ))
                .Do(pc => pc.Permissions = new List<PermissionEntity>())
                .ToArray();

            Permissions = Enumerable.Empty<PermissionEntity>()
                .Append(new PermissionEntity(   permissionId: 1,    categoryId: 1,  name: "ManagePermissions",  description: "Allows management of application permissions" ))
                .Append(new PermissionEntity(   permissionId: 2,    categoryId: 1,  name: "ManageRoles",        description: "Allows management of application roles"       ))
                .Append(new PermissionEntity(   permissionId: 3,    categoryId: 1,  name: "ManageUsers",        description: "Allows management of application users"       ))
                .Do(p => p.Category = PermissionCategories.First(pc => pc.Id == p.CategoryId))
                .Do(p => p.Category.Permissions.Add(p))
                .ToArray();


            Users = Enumerable.Empty<UserEntity>()
                .Append(new UserEntity( id: 1,  username: "User 1", discriminator: "0001",  avatarHash: "00001",    firstSeen: DateTimeOffset.Parse("2019-01-01"),  lastSeen: DateTimeOffset.Parse("2019-01-05")    ))
                .Append(new UserEntity( id: 2,  username: "User 2", discriminator: "0002",  avatarHash: "00002",    firstSeen: DateTimeOffset.Parse("2019-01-03"),  lastSeen: DateTimeOffset.Parse("2019-01-07")    ))
                .Append(new UserEntity( id: 3,  username: "User 3", discriminator: "0003",  avatarHash: "00003",    firstSeen: DateTimeOffset.Parse("2019-01-09"),  lastSeen: DateTimeOffset.Parse("2019-01-09")    ))
                .Do(u => u.PermissionMappings = new List<UserPermissionMappingEntity>())
                .Do(u => u.RoleMappings = new List<UserRoleMappingEntity>())
                .ToArray();


            AdministrationActionCategories = Enumerable.Empty<AdministrationActionCategoryEntity>()
                .Append(new AdministrationActionCategoryEntity( id: 1,  name: "RoleManagement"  ))
                .Append(new AdministrationActionCategoryEntity( id: 2,  name: "UserManagement"  ))
                .ToArray();

            AdministrationActionTypes = Enumerable.Empty<AdministrationActionTypeEntity>()
                .Append(new AdministrationActionTypeEntity( id: 1,  categoryId: 1,  name: "RoleCreated"         ))
                .Append(new AdministrationActionTypeEntity( id: 2,  categoryId: 1,  name: "RoleModified"        ))
                .Append(new AdministrationActionTypeEntity( id: 3,  categoryId: 1,  name: "RoleDeleted"         ))
                .Append(new AdministrationActionTypeEntity( id: 4,  categoryId: 1,  name: "RoleRestored"        ))
                .Append(new AdministrationActionTypeEntity( id: 20, categoryId: 2,  name: "UserCreated"         ))
                .Append(new AdministrationActionTypeEntity( id: 21, categoryId: 2,  name: "UserModified"        ))
                .Append(new AdministrationActionTypeEntity( id: 22, categoryId: 2,  name: "DefaultsModified"    ))
                .Do(aat => aat.Category = AdministrationActionCategories.First(aac => aac.Id == aat.CategoryId))
                .ToArray();

            AdministrationActions = Enumerable.Empty<AdministrationActionEntity>()
                .Append(new AdministrationActionEntity( id: 1,  typeId: 20, performed: DateTimeOffset.Parse("2019-01-01"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 2,  typeId: 1,  performed: DateTimeOffset.Parse("2019-01-02"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 3,  typeId: 20, performed: DateTimeOffset.Parse("2019-01-03"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 4,  typeId: 2,  performed: DateTimeOffset.Parse("2019-01-04"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 5,  typeId: 21, performed: DateTimeOffset.Parse("2019-01-05"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 6,  typeId: 2,  performed: DateTimeOffset.Parse("2019-01-06"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 7,  typeId: 21, performed: DateTimeOffset.Parse("2019-01-07"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 8,  typeId: 2,  performed: DateTimeOffset.Parse("2019-01-08"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 9,  typeId: 20, performed: DateTimeOffset.Parse("2019-01-09"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 10, typeId: 1,  performed: DateTimeOffset.Parse("2019-01-10"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 11, typeId: 2,  performed: DateTimeOffset.Parse("2019-01-11"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 12, typeId: 2,  performed: DateTimeOffset.Parse("2019-01-12"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 13, typeId: 2,  performed: DateTimeOffset.Parse("2019-01-13"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 14, typeId: 1,  performed: DateTimeOffset.Parse("2019-01-14"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 15, typeId: 3,  performed: DateTimeOffset.Parse("2019-01-15"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 16, typeId: 3,  performed: DateTimeOffset.Parse("2019-01-16"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 17, typeId: 4,  performed: DateTimeOffset.Parse("2019-01-17"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 18, typeId: 22, performed: DateTimeOffset.Parse("2019-01-18"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 19, typeId: 22, performed: DateTimeOffset.Parse("2019-01-19"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 20, typeId: 22, performed: DateTimeOffset.Parse("2019-01-20"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 21, typeId: 22, performed: DateTimeOffset.Parse("2019-01-21"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 22, typeId: 22, performed: DateTimeOffset.Parse("2019-01-22"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 23, typeId: 22, performed: DateTimeOffset.Parse("2019-01-23"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 24, typeId: 22, performed: DateTimeOffset.Parse("2019-01-24"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 25, typeId: 22, performed: DateTimeOffset.Parse("2019-01-25"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 26, typeId: 22, performed: DateTimeOffset.Parse("2019-01-26"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 27, typeId: 21, performed: DateTimeOffset.Parse("2019-01-27"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 28, typeId: 21, performed: DateTimeOffset.Parse("2019-01-28"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 29, typeId: 21, performed: DateTimeOffset.Parse("2019-01-29"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 30, typeId: 21, performed: DateTimeOffset.Parse("2019-01-30"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 31, typeId: 21, performed: DateTimeOffset.Parse("2019-01-31"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 32, typeId: 21, performed: DateTimeOffset.Parse("2019-02-01"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 33, typeId: 21, performed: DateTimeOffset.Parse("2019-02-02"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 34, typeId: 21, performed: DateTimeOffset.Parse("2019-02-03"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 35, typeId: 21, performed: DateTimeOffset.Parse("2019-02-04"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 36, typeId: 21, performed: DateTimeOffset.Parse("2019-02-05"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 37, typeId: 21, performed: DateTimeOffset.Parse("2019-02-06"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 38, typeId: 21, performed: DateTimeOffset.Parse("2019-02-07"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 39, typeId: 21, performed: DateTimeOffset.Parse("2019-02-08"),  performedById: 1    ))
                .Do(aa => aa.Type = AdministrationActionTypes.First(aat => aat.Id == aa.TypeId))
                .Do(aa => aa.PerformedBy = Users.First(u => u.Id == aa.PerformedById))
                .ToArray();


            AuthenticationTickets = Enumerable.Empty<AuthenticationTicketEntity>()
                .Append(new AuthenticationTicketEntity( id: 1,  userId: 1,  creationId: 1,  deletionId: 5       ))
                .Append(new AuthenticationTicketEntity( id: 2,  userId: 2,  creationId: 3,  deletionId: 7       ))
                .Append(new AuthenticationTicketEntity( id: 3,  userId: 1,  creationId: 5,  deletionId: null    ))
                .Append(new AuthenticationTicketEntity( id: 4,  userId: 3,  creationId: 7,  deletionId: null    ))
                .Append(new AuthenticationTicketEntity( id: 5,  userId: 2,  creationId: 9,  deletionId: null    ))
                .Do(at => at.User = Users.First(u => u.Id == at.UserId))
                .Do(at => at.Creation = AdministrationActions.First(aa => aa.Id == at.CreationId))
                .Do(at => at.Deletion = (at.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == at.DeletionId))
                .ToArray();


            Roles = Enumerable.Empty<RoleEntity>()
                .Append(new RoleEntity( id: 1   ))
                .Append(new RoleEntity( id: 2   ))
                .Append(new RoleEntity( id: 3   ))
                .Do(r => r.PermissionMappings = new List<RolePermissionMappingEntity>())
                .ToArray();

            RoleVersions = Enumerable.Empty<RoleVersionEntity>()
                .Append(new RoleVersionEntity(  id: 1,  roleId: 1,  name: "Role 1",     isDeleted: false,   actionId: 2,    nextVersionId: null,    previousVersionId: null ))
                .Append(new RoleVersionEntity(  id: 2,  roleId: 2,  name: "Role 2",     isDeleted: false,   actionId: 10,   nextVersionId: 3,       previousVersionId: null ))
                .Append(new RoleVersionEntity(  id: 3,  roleId: 2,  name: "Role 2a",    isDeleted: false,   actionId: 12,   nextVersionId: 5,       previousVersionId: 2    ))
                .Append(new RoleVersionEntity(  id: 4,  roleId: 3,  name: "Role 3",     isDeleted: false,   actionId: 14,   nextVersionId: 6,       previousVersionId: null ))
                .Append(new RoleVersionEntity(  id: 5,  roleId: 2,  name: "Role 2a",    isDeleted: true,    actionId: 15,   nextVersionId: null,    previousVersionId: 3    ))
                .Append(new RoleVersionEntity(  id: 6,  roleId: 3,  name: "Role 3a",    isDeleted: true,    actionId: 16,   nextVersionId: 7,       previousVersionId: 4    ))
                .Append(new RoleVersionEntity(  id: 7,  roleId: 3,  name: "Role 3",     isDeleted: false,   actionId: 17,   nextVersionId: null,    previousVersionId: 7    ))
                .Do(rv => rv.Role = Roles.First(r => r.Id == rv.RoleId))
                .Do(rv => rv.Action = AdministrationActions.First(aa => aa.Id == rv.ActionId))
                .ToArray();
            RoleVersions
                .Do(rv => rv.PreviousVersion = (rv.PreviousVersionId is null) ? null : RoleVersions.First(pv => pv.Id == rv.PreviousVersionId))
                .Do(rv => rv.NextVersion = (rv.NextVersionId is null) ? null : RoleVersions.First(nv => nv.Id == rv.NextVersionId))
                .Enumerate();

            RolePermissionMappings = Enumerable.Empty<RolePermissionMappingEntity>()
                .Append(new RolePermissionMappingEntity(    id: 1,  roleId: 1,  permissionId: 1,    creationId: 4,  deletionId: null    ))
                .Append(new RolePermissionMappingEntity(    id: 2,  roleId: 2,  permissionId: 2,    creationId: 6,  deletionId: 8       ))
                .Append(new RolePermissionMappingEntity(    id: 3,  roleId: 2,  permissionId: 3,    creationId: 8,  deletionId: null    ))
                .Append(new RolePermissionMappingEntity(    id: 4,  roleId: 3,  permissionId: 1,    creationId: 11, deletionId: null    ))
                .Append(new RolePermissionMappingEntity(    id: 5,  roleId: 3,  permissionId: 2,    creationId: 11, deletionId: 13      ))
                .Append(new RolePermissionMappingEntity(    id: 6,  roleId: 3,  permissionId: 3,    creationId: 11, deletionId: null    ))
                .Do(rpm => rpm.Role = Roles.First(r => r.Id == rpm.RoleId))
                .Do(rpm => rpm.Role.PermissionMappings.Add(rpm))
                .Do(rpm => rpm.Permission = Permissions.First(p => p.PermissionId == rpm.PermissionId))
                .Do(rpm => rpm.Creation = AdministrationActions.First(aa => aa.Id == rpm.CreationId))
                .Do(rpm => rpm.Deletion = (rpm.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == rpm.DeletionId))
                .ToArray();


            DefaultPermissionMappings = Enumerable.Empty<DefaultPermissionMappingEntity>()
                .Append(new DefaultPermissionMappingEntity( id: 1,  permissionId: 1,    creationId: 18, deletionId: null    ))
                .Append(new DefaultPermissionMappingEntity( id: 2,  permissionId: 2,    creationId: 20, deletionId: 21      ))
                .Append(new DefaultPermissionMappingEntity( id: 3,  permissionId: 3,    creationId: 21, deletionId: 22      ))
                .Append(new DefaultPermissionMappingEntity( id: 4,  permissionId: 2,    creationId: 23, deletionId: null    ))
                .Do(dpm => dpm.Permission = Permissions.First(p => p.PermissionId == dpm.PermissionId))
                .Do(dpm => dpm.Creation = AdministrationActions.First(aa => aa.Id == dpm.CreationId))
                .Do(dpm => dpm.Deletion = (dpm.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == dpm.DeletionId))
                .ToArray();

            DefaultRoleMappings = Enumerable.Empty<DefaultRoleMappingEntity>()
                .Append(new DefaultRoleMappingEntity(   id: 1,  roleId: 1,  creationId: 19, deletionId: null    ))
                .Append(new DefaultRoleMappingEntity(   id: 2,  roleId: 2,  creationId: 21, deletionId: 24      ))
                .Append(new DefaultRoleMappingEntity(   id: 3,  roleId: 3,  creationId: 24, deletionId: 26      ))
                .Append(new DefaultRoleMappingEntity(   id: 4,  roleId: 2,  creationId: 25, deletionId: null    ))
                .Do(drm => drm.Role = Roles.First(r => r.Id == drm.RoleId))
                .Do(drm => drm.Creation = AdministrationActions.First(aa => aa.Id == drm.CreationId))
                .Do(drm => drm.Deletion = (drm.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == drm.DeletionId))
                .ToArray();

            UserPermissionMappings = Enumerable.Empty<UserPermissionMappingEntity>()
                .Append(new UserPermissionMappingEntity(    id: 1,  userId: 1,  permissionId: 3,    isDenied: false,    creationId: 27, deletionId: 31      ))
                .Append(new UserPermissionMappingEntity(    id: 2,  userId: 3,  permissionId: 1,    isDenied: true,     creationId: 28, deletionId: null    ))
                .Append(new UserPermissionMappingEntity(    id: 3,  userId: 3,  permissionId: 2,    isDenied: true,     creationId: 28, deletionId: null    ))
                .Append(new UserPermissionMappingEntity(    id: 4,  userId: 2,  permissionId: 3,    isDenied: false,    creationId: 29, deletionId: null    ))
                .Append(new UserPermissionMappingEntity(    id: 5,  userId: 2,  permissionId: 2,    isDenied: false,    creationId: 29, deletionId: 32      ))
                .Append(new UserPermissionMappingEntity(    id: 6,  userId: 2,  permissionId: 1,    isDenied: true,     creationId: 30, deletionId: null    ))
                .Append(new UserPermissionMappingEntity(    id: 7,  userId: 1,  permissionId: 2,    isDenied: false,    creationId: 31, deletionId: null    ))
                .Append(new UserPermissionMappingEntity(    id: 8,  userId: 1,  permissionId: 1,    isDenied: false,    creationId: 31, deletionId: null    ))
                .Do(upm => upm.User = Users.First(r => r.Id == upm.UserId))
                .Do(upm => upm.User.PermissionMappings.Add(upm))
                .Do(upm => upm.Permission = Permissions.First(p => p.PermissionId == upm.PermissionId))
                .Do(upm => upm.Creation = AdministrationActions.First(aa => aa.Id == upm.CreationId))
                .Do(upm => upm.Deletion = (upm.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == upm.DeletionId))
                .ToArray();

            UserRoleMappings = Enumerable.Empty<UserRoleMappingEntity>()
                .Append(new UserRoleMappingEntity(  id: 1,  userId: 3,  roleId: 1,  creationId: 33, deletionId: 38      ))
                .Append(new UserRoleMappingEntity(  id: 2,  userId: 3,  roleId: 3,  creationId: 34, deletionId: 38      ))
                .Append(new UserRoleMappingEntity(  id: 3,  userId: 2,  roleId: 3,  creationId: 35, deletionId: null    ))
                .Append(new UserRoleMappingEntity(  id: 4,  userId: 2,  roleId: 1,  creationId: 35, deletionId: null    ))
                .Append(new UserRoleMappingEntity(  id: 5,  userId: 1,  roleId: 2,  creationId: 36, deletionId: null    ))
                .Append(new UserRoleMappingEntity(  id: 6,  userId: 1,  roleId: 1,  creationId: 37, deletionId: 39      ))
                .Do(urm => urm.User = Users.First(r => r.Id == urm.UserId))
                .Do(urm => urm.User.RoleMappings.Add(urm))
                .Do(urm => urm.Role = Roles.First(p => p.Id == urm.RoleId))
                .Do(urm => urm.Creation = AdministrationActions.First(aa => aa.Id == urm.CreationId))
                .Do(urm => urm.Deletion = (urm.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == urm.DeletionId))
                .ToArray();
        }

        #region Administration

        public readonly IReadOnlyList<AdministrationActionCategoryEntity> AdministrationActionCategories;
        
        public readonly IReadOnlyList<AdministrationActionTypeEntity> AdministrationActionTypes;

        public readonly IReadOnlyList<AdministrationActionEntity> AdministrationActions;

        #endregion Administration

        #region Authentication

        public readonly IReadOnlyList<AuthenticationTicketEntity> AuthenticationTickets;

        #endregion Authentication

        #region Permission

        public readonly IReadOnlyList<PermissionCategoryEntity> PermissionCategories;

        public readonly IReadOnlyList<PermissionEntity> Permissions;

        #endregion Permission

        #region Roles

        public readonly IReadOnlyList<RolePermissionMappingEntity> RolePermissionMappings;
        
        public readonly IReadOnlyList<RoleVersionEntity> RoleVersions;

        public readonly IReadOnlyList<RoleEntity> Roles;

        #endregion Roles

        #region Users

        public readonly IReadOnlyList<DefaultPermissionMappingEntity> DefaultPermissionMappings;

        public readonly IReadOnlyList<DefaultRoleMappingEntity> DefaultRoleMappings;

        public readonly IReadOnlyList<UserPermissionMappingEntity> UserPermissionMappings;
        
        public readonly IReadOnlyList<UserRoleMappingEntity> UserRoleMappings;

        public readonly IReadOnlyList<UserEntity> Users;

        #endregion Users
    }
}
