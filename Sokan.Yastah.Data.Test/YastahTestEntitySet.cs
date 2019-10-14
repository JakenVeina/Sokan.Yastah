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
                .Append(new PermissionCategoryEntity() { Id = 1, Name = "Administration", Description = "Permissions related to administration of the application" })
                .Do(pc => pc.Permissions = new List<PermissionEntity>())
                .ToArray();

            Permissions = Enumerable.Empty<PermissionEntity>()
                .Append(new PermissionEntity() {    PermissionId = 1,   CategoryId = 1, Name = "ManagePermissions", Description = "Allows management of application permissions"    })
                .Append(new PermissionEntity() {    PermissionId = 2,   CategoryId = 1, Name = "ManageRoles",       Description = "Allows management of application roles"          })
                .Append(new PermissionEntity() {    PermissionId = 3,   CategoryId = 1, Name = "ManageUsers",       Description = "Allows management of application users"          })
                .Do(p => p.Category = PermissionCategories.First(pc => pc.Id == p.CategoryId))
                .Do(p => p.Category.Permissions.Add(p))
                .ToArray();


            Users = Enumerable.Empty<UserEntity>()
                .Append(new UserEntity() { Id = 1, Username = "User 1", Discriminator = "0001", AvatarHash = "00001", FirstSeen = DateTimeOffset.Parse("2019-01-01"), LastSeen = DateTimeOffset.Parse("2019-01-05") })
                .Append(new UserEntity() { Id = 2, Username = "User 2", Discriminator = "0002", AvatarHash = "00002", FirstSeen = DateTimeOffset.Parse("2019-01-03"), LastSeen = DateTimeOffset.Parse("2019-01-07") })
                .Append(new UserEntity() { Id = 3, Username = "User 3", Discriminator = "0003", AvatarHash = "00003", FirstSeen = DateTimeOffset.Parse("2019-01-09"), LastSeen = DateTimeOffset.Parse("2019-01-09") })
                .Do(u => u.PermissionMappings = new List<UserPermissionMappingEntity>())
                .Do(u => u.RoleMappings = new List<UserRoleMappingEntity>())
                .ToArray();


            AdministrationActionCategories = Enumerable.Empty<AdministrationActionCategoryEntity>()
                .Append(new AdministrationActionCategoryEntity() {  Id = 1, Name = "RoleManagement" })
                .Append(new AdministrationActionCategoryEntity() {  Id = 2, Name = "UserManagement" })
                .ToArray();

            AdministrationActionTypes = Enumerable.Empty<AdministrationActionTypeEntity>()
                .Append(new AdministrationActionTypeEntity() {  Id = 1,     CategoryId = 1, Name = "RoleCreated"        })
                .Append(new AdministrationActionTypeEntity() {  Id = 2,     CategoryId = 1, Name = "RoleModified"       })
                .Append(new AdministrationActionTypeEntity() {  Id = 3,     CategoryId = 1, Name = "RoleDeleted"        })
                .Append(new AdministrationActionTypeEntity() {  Id = 4,     CategoryId = 1, Name = "RoleRestored"       })
                .Append(new AdministrationActionTypeEntity() {  Id = 20,    CategoryId = 2, Name = "UserCreated"        })
                .Append(new AdministrationActionTypeEntity() {  Id = 21,    CategoryId = 2, Name = "UserModified"       })
                .Append(new AdministrationActionTypeEntity() {  Id = 22,    CategoryId = 2, Name = "DefaultsModified"   })
                .Do(aat => aat.Category = AdministrationActionCategories.First(aac => aac.Id == aat.CategoryId))
                .ToArray();

            AdministrationActions = Enumerable.Empty<AdministrationActionEntity>()
                .Append(new AdministrationActionEntity() {  Id = 1,     TypeId = 20,    Performed = DateTimeOffset.Parse("2019-01-01"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 2,     TypeId = 1,     Performed = DateTimeOffset.Parse("2019-01-02"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 3,     TypeId = 20,    Performed = DateTimeOffset.Parse("2019-01-03"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 4,     TypeId = 2,     Performed = DateTimeOffset.Parse("2019-01-04"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 5,     TypeId = 21,    Performed = DateTimeOffset.Parse("2019-01-05"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 6,     TypeId = 2,     Performed = DateTimeOffset.Parse("2019-01-06"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 7,     TypeId = 21,    Performed = DateTimeOffset.Parse("2019-01-07"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 8,     TypeId = 2,     Performed = DateTimeOffset.Parse("2019-01-08"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 9,     TypeId = 20,    Performed = DateTimeOffset.Parse("2019-01-09"), PerformedById = 3   })
                .Append(new AdministrationActionEntity() {  Id = 10,    TypeId = 1,     Performed = DateTimeOffset.Parse("2019-01-10"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 11,    TypeId = 2,     Performed = DateTimeOffset.Parse("2019-01-11"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 12,    TypeId = 2,     Performed = DateTimeOffset.Parse("2019-01-12"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 13,    TypeId = 2,     Performed = DateTimeOffset.Parse("2019-01-13"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 14,    TypeId = 1,     Performed = DateTimeOffset.Parse("2019-01-14"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 15,    TypeId = 3,     Performed = DateTimeOffset.Parse("2019-01-15"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 16,    TypeId = 3,     Performed = DateTimeOffset.Parse("2019-01-16"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 17,    TypeId = 4,     Performed = DateTimeOffset.Parse("2019-01-17"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 18,    TypeId = 22,    Performed = DateTimeOffset.Parse("2019-01-18"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 19,    TypeId = 22,    Performed = DateTimeOffset.Parse("2019-01-19"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 20,    TypeId = 22,    Performed = DateTimeOffset.Parse("2019-01-20"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 21,    TypeId = 22,    Performed = DateTimeOffset.Parse("2019-01-21"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 22,    TypeId = 22,    Performed = DateTimeOffset.Parse("2019-01-22"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 23,    TypeId = 22,    Performed = DateTimeOffset.Parse("2019-01-23"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 24,    TypeId = 22,    Performed = DateTimeOffset.Parse("2019-01-24"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 25,    TypeId = 22,    Performed = DateTimeOffset.Parse("2019-01-25"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 26,    TypeId = 22,    Performed = DateTimeOffset.Parse("2019-01-26"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 27,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-01-27"), PerformedById = 3   })
                .Append(new AdministrationActionEntity() {  Id = 28,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-01-28"), PerformedById = 3   })
                .Append(new AdministrationActionEntity() {  Id = 29,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-01-29"), PerformedById = 3   })
                .Append(new AdministrationActionEntity() {  Id = 30,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-01-30"), PerformedById = 3   })
                .Append(new AdministrationActionEntity() {  Id = 31,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-01-31"), PerformedById = 3   })
                .Append(new AdministrationActionEntity() {  Id = 32,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-02-01"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 33,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-02-02"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 34,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-02-03"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 35,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-02-04"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 36,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-02-05"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 37,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-02-06"), PerformedById = 2   })
                .Append(new AdministrationActionEntity() {  Id = 38,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-02-07"), PerformedById = 1   })
                .Append(new AdministrationActionEntity() {  Id = 39,    TypeId = 21,    Performed = DateTimeOffset.Parse("2019-02-08"), PerformedById = 1   })
                .Do(aa => aa.Type = AdministrationActionTypes.First(aat => aat.Id == aa.TypeId))
                .Do(aa => aa.PerformedBy = Users.First(u => u.Id == aa.PerformedById))
                .ToArray();


            AuthenticationTickets = Enumerable.Empty<AuthenticationTicketEntity>()
                .Append(new AuthenticationTicketEntity() {  Id = 1, UserId = 1, CreationId = 1, DeletionId = 5      })
                .Append(new AuthenticationTicketEntity() {  Id = 2, UserId = 2, CreationId = 3, DeletionId = 7      })
                .Append(new AuthenticationTicketEntity() {  Id = 3, UserId = 1, CreationId = 5, DeletionId = null   })
                .Append(new AuthenticationTicketEntity() {  Id = 4, UserId = 3, CreationId = 7, DeletionId = null   })
                .Append(new AuthenticationTicketEntity() {  Id = 5, UserId = 2, CreationId = 9, DeletionId = null   })
                .Do(at => at.User = Users.First(u => u.Id == at.UserId))
                .Do(at => at.Creation = AdministrationActions.First(aa => aa.Id == at.CreationId))
                .Do(at => at.Deletion = (at.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == at.DeletionId))
                .ToArray();


            Roles = Enumerable.Empty<RoleEntity>()
                .Append(new RoleEntity() { Id = 1 })
                .Append(new RoleEntity() { Id = 2 })
                .Append(new RoleEntity() { Id = 3 })
                .Do(r => r.PermissionMappings = new List<RolePermissionMappingEntity>())
                .ToArray();

            RoleVersions = Enumerable.Empty<RoleVersionEntity>()
                .Append(new RoleVersionEntity() {   Id = 1, RoleId = 1, Name = "Role 1",    IsDeleted = false,  ActionId = 2,   NextVersionId = null,   PreviousVersionId = null    })
                .Append(new RoleVersionEntity() {   Id = 2, RoleId = 2, Name = "Role 2",    IsDeleted = false,  ActionId = 10,  NextVersionId = 3,      PreviousVersionId = null    })
                .Append(new RoleVersionEntity() {   Id = 3, RoleId = 2, Name = "Role 2a",   IsDeleted = false,  ActionId = 12,  NextVersionId = 5,      PreviousVersionId = 2       })
                .Append(new RoleVersionEntity() {   Id = 4, RoleId = 3, Name = "Role 3",    IsDeleted = false,  ActionId = 14,  NextVersionId = 6,      PreviousVersionId = null    })
                .Append(new RoleVersionEntity() {   Id = 5, RoleId = 2, Name = "Role 2a",   IsDeleted = true,   ActionId = 15,  NextVersionId = null,   PreviousVersionId = 3       })
                .Append(new RoleVersionEntity() {   Id = 6, RoleId = 3, Name = "Role 3a",   IsDeleted = true,   ActionId = 16,  NextVersionId = 7,      PreviousVersionId = 4       })
                .Append(new RoleVersionEntity() {   Id = 7, RoleId = 3, Name = "Role 3",    IsDeleted = false,  ActionId = 17,  NextVersionId = null,   PreviousVersionId = 7       })
                .Do(rv => rv.Role = Roles.First(r => r.Id == rv.RoleId))
                .Do(rv => rv.Action = AdministrationActions.First(aa => aa.Id == rv.ActionId))
                .ToArray();
            RoleVersions
                .Do(rv => rv.PreviousVersion = (rv.PreviousVersionId is null) ? null : RoleVersions.First(pv => pv.Id == rv.PreviousVersionId))
                .Do(rv => rv.NextVersion = (rv.NextVersionId is null) ? null : RoleVersions.First(nv => nv.Id == rv.NextVersionId))
                .Enumerate();

            RolePermissionMappings = Enumerable.Empty<RolePermissionMappingEntity>()
                .Append(new RolePermissionMappingEntity() { Id = 1, RoleId = 1, PermissionId = 1,   CreationId = 4,     DeletionId = null   })
                .Append(new RolePermissionMappingEntity() { Id = 2, RoleId = 2, PermissionId = 2,   CreationId = 6,     DeletionId = 8      })
                .Append(new RolePermissionMappingEntity() { Id = 3, RoleId = 2, PermissionId = 3,   CreationId = 8,     DeletionId = null   })
                .Append(new RolePermissionMappingEntity() { Id = 4, RoleId = 3, PermissionId = 1,   CreationId = 11,    DeletionId = null   })
                .Append(new RolePermissionMappingEntity() { Id = 5, RoleId = 3, PermissionId = 2,   CreationId = 11,    DeletionId = 13     })
                .Append(new RolePermissionMappingEntity() { Id = 6, RoleId = 3, PermissionId = 3,   CreationId = 11,    DeletionId = null   })
                .Do(rpm => rpm.Role = Roles.First(r => r.Id == rpm.RoleId))
                .Do(rpm => rpm.Role.PermissionMappings.Add(rpm))
                .Do(rpm => rpm.Permission = Permissions.First(p => p.PermissionId == rpm.PermissionId))
                .Do(rpm => rpm.Creation = AdministrationActions.First(aa => aa.Id == rpm.CreationId))
                .Do(rpm => rpm.Deletion = (rpm.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == rpm.DeletionId))
                .ToArray();


            DefaultPermissionMappings = Enumerable.Empty<DefaultPermissionMappingEntity>()
                .Append(new DefaultPermissionMappingEntity() {  Id = 1, PermissionId = 1,   CreationId = 18,    DeletionId = null   })
                .Append(new DefaultPermissionMappingEntity() {  Id = 2, PermissionId = 2,   CreationId = 20,    DeletionId = 21     })
                .Append(new DefaultPermissionMappingEntity() {  Id = 3, PermissionId = 3,   CreationId = 21,    DeletionId = 22     })
                .Append(new DefaultPermissionMappingEntity() {  Id = 4, PermissionId = 2,   CreationId = 23,    DeletionId = null   })
                .Do(dpm => dpm.Permission = Permissions.First(p => p.PermissionId == dpm.PermissionId))
                .Do(dpm => dpm.Creation = AdministrationActions.First(aa => aa.Id == dpm.CreationId))
                .Do(dpm => dpm.Deletion = (dpm.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == dpm.DeletionId))
                .ToArray();

            DefaultRoleMappings = Enumerable.Empty<DefaultRoleMappingEntity>()
                .Append(new DefaultRoleMappingEntity() {    Id = 1, RoleId = 1, CreationId = 19,    DeletionId = null   })
                .Append(new DefaultRoleMappingEntity() {    Id = 2, RoleId = 2, CreationId = 21,    DeletionId = 24     })
                .Append(new DefaultRoleMappingEntity() {    Id = 3, RoleId = 3, CreationId = 24,    DeletionId = 26     })
                .Append(new DefaultRoleMappingEntity() {    Id = 4, RoleId = 2, CreationId = 25,    DeletionId = null   })
                .Do(drm => drm.Role = Roles.First(r => r.Id == drm.RoleId))
                .Do(drm => drm.Creation = AdministrationActions.First(aa => aa.Id == drm.CreationId))
                .Do(drm => drm.Deletion = (drm.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == drm.DeletionId))
                .ToArray();

            UserPermissionMappings = Enumerable.Empty<UserPermissionMappingEntity>()
                .Append(new UserPermissionMappingEntity() { Id = 1, UserId = 1, PermissionId = 3,   IsDenied = false,   CreationId = 27,    DeletionId = 31     })
                .Append(new UserPermissionMappingEntity() { Id = 2, UserId = 3, PermissionId = 1,   IsDenied = true,    CreationId = 28,    DeletionId = null   })
                .Append(new UserPermissionMappingEntity() { Id = 3, UserId = 3, PermissionId = 2,   IsDenied = true,    CreationId = 28,    DeletionId = null   })
                .Append(new UserPermissionMappingEntity() { Id = 4, UserId = 2, PermissionId = 3,   IsDenied = false,   CreationId = 29,    DeletionId = null   })
                .Append(new UserPermissionMappingEntity() { Id = 5, UserId = 2, PermissionId = 2,   IsDenied = false,   CreationId = 29,    DeletionId = 32     })
                .Append(new UserPermissionMappingEntity() { Id = 6, UserId = 2, PermissionId = 1,   IsDenied = true,    CreationId = 30,    DeletionId = null   })
                .Append(new UserPermissionMappingEntity() { Id = 7, UserId = 1, PermissionId = 2,   IsDenied = false,   CreationId = 31,    DeletionId = null   })
                .Append(new UserPermissionMappingEntity() { Id = 8, UserId = 1, PermissionId = 1,   IsDenied = false,   CreationId = 31,    DeletionId = null   })
                .Do(upm => upm.User = Users.First(r => r.Id == upm.UserId))
                .Do(upm => upm.User.PermissionMappings.Add(upm))
                .Do(upm => upm.Permission = Permissions.First(p => p.PermissionId == upm.PermissionId))
                .Do(upm => upm.Creation = AdministrationActions.First(aa => aa.Id == upm.CreationId))
                .Do(upm => upm.Deletion = (upm.DeletionId is null) ? null : AdministrationActions.First(aa => aa.Id == upm.DeletionId))
                .ToArray();

            UserRoleMappings = Enumerable.Empty<UserRoleMappingEntity>()
                .Append(new UserRoleMappingEntity() {   Id = 1, UserId = 3, RoleId = 1, CreationId = 33,    DeletionId = 38     })
                .Append(new UserRoleMappingEntity() {   Id = 2, UserId = 3, RoleId = 3, CreationId = 34,    DeletionId = 38     })
                .Append(new UserRoleMappingEntity() {   Id = 3, UserId = 2, RoleId = 3, CreationId = 35,    DeletionId = null   })
                .Append(new UserRoleMappingEntity() {   Id = 4, UserId = 2, RoleId = 1, CreationId = 35,    DeletionId = null   })
                .Append(new UserRoleMappingEntity() {   Id = 5, UserId = 1, RoleId = 2, CreationId = 36,    DeletionId = null   })
                .Append(new UserRoleMappingEntity() {   Id = 6, UserId = 1, RoleId = 1, CreationId = 37,    DeletionId = 39     })
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
