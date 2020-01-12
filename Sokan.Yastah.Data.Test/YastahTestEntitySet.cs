using System;
using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Authentication;
using Sokan.Yastah.Data.Characters;
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
                .Append(new PermissionCategoryEntity(   id: (int)PermissionCategory.Administration, name: nameof(PermissionCategory.Administration),    description: "Permissions related to administration of the application" ))
                .Do(pc => pc.Permissions = new List<PermissionEntity>())
                .ToArray();

            Permissions = Enumerable.Empty<PermissionEntity>()
                .Append(new PermissionEntity(   permissionId: (int)AdministrationPermission.ManagePermissions,  categoryId: (int)PermissionCategory.Administration,  name: nameof(AdministrationPermission.ManagePermissions),  description: "Allows management of application permissions" ))
                .Append(new PermissionEntity(   permissionId: (int)AdministrationPermission.ManageRoles,        categoryId: (int)PermissionCategory.Administration,  name: nameof(AdministrationPermission.ManageRoles),        description: "Allows management of application roles"       ))
                .Append(new PermissionEntity(   permissionId: (int)AdministrationPermission.ManageUsers,        categoryId: (int)PermissionCategory.Administration,  name: nameof(AdministrationPermission.ManageUsers),        description: "Allows management of application users"       ))
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
                .Append(new AdministrationActionCategoryEntity( id: (int)AdministrationActionCategory.RoleManagement,       name: nameof(AdministrationActionCategory.RoleManagement)       ))
                .Append(new AdministrationActionCategoryEntity( id: (int)AdministrationActionCategory.UserManagement,       name: nameof(AdministrationActionCategory.UserManagement)       ))
                .Append(new AdministrationActionCategoryEntity( id: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(AdministrationActionCategory.CharacterManagement)  ))
                .ToArray();

            AdministrationActionTypes = Enumerable.Empty<AdministrationActionTypeEntity>()
                .Append(new AdministrationActionTypeEntity( id: (int)RoleManagementAdministrationActionType.RoleCreated,            categoryId: (int)AdministrationActionCategory.RoleManagement,       name: nameof(RoleManagementAdministrationActionType.RoleCreated)            ))
                .Append(new AdministrationActionTypeEntity( id: (int)RoleManagementAdministrationActionType.RoleModified,           categoryId: (int)AdministrationActionCategory.RoleManagement,       name: nameof(RoleManagementAdministrationActionType.RoleModified)           ))
                .Append(new AdministrationActionTypeEntity( id: (int)RoleManagementAdministrationActionType.RoleDeleted,            categoryId: (int)AdministrationActionCategory.RoleManagement,       name: nameof(RoleManagementAdministrationActionType.RoleDeleted)            ))
                .Append(new AdministrationActionTypeEntity( id: (int)RoleManagementAdministrationActionType.RoleRestored,           categoryId: (int)AdministrationActionCategory.RoleManagement,       name: nameof(RoleManagementAdministrationActionType.RoleRestored)           ))
                .Append(new AdministrationActionTypeEntity( id: (int)UserManagementAdministrationActionType.DefaultsModified,       categoryId: (int)AdministrationActionCategory.UserManagement,       name: nameof(UserManagementAdministrationActionType.DefaultsModified)       ))
                .Append(new AdministrationActionTypeEntity( id: (int)UserManagementAdministrationActionType.UserCreated,            categoryId: (int)AdministrationActionCategory.UserManagement,       name: nameof(UserManagementAdministrationActionType.UserCreated)            ))
                .Append(new AdministrationActionTypeEntity( id: (int)UserManagementAdministrationActionType.UserModified,           categoryId: (int)AdministrationActionCategory.UserManagement,       name: nameof(UserManagementAdministrationActionType.UserModified)           ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.GuildCreated,      categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.GuildCreated)      ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.GuildModified,     categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.GuildModified)     ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.GuildDeleted,      categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.GuildDeleted)      ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.GuildRestored,     categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.GuildRestored)     ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.DivisionCreated,   categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.DivisionCreated)   ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.DivisionModified,  categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.DivisionModified)  ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.DivisionDeleted,   categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.DivisionDeleted)   ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.DivisionRestored,  categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.DivisionRestored)  ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.CharacterCreated,  categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.CharacterCreated)  ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.CharacterModified, categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.CharacterModified) ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.CharacterDeleted,  categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.CharacterDeleted)  ))
                .Append(new AdministrationActionTypeEntity( id: (int)CharacterManagementAdministrationActionType.CharacterRestored, categoryId: (int)AdministrationActionCategory.CharacterManagement,  name: nameof(CharacterManagementAdministrationActionType.CharacterRestored) ))
                .Do(aat => aat.Category = AdministrationActionCategories.First(aac => aac.Id == aat.CategoryId))
                .ToArray();

            AdministrationActions = Enumerable.Empty<AdministrationActionEntity>()
                .Append(new AdministrationActionEntity( id: 1,  typeId: (int)UserManagementAdministrationActionType.UserCreated,            performed: DateTimeOffset.Parse("2019-01-01"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 2,  typeId: (int)RoleManagementAdministrationActionType.RoleCreated,            performed: DateTimeOffset.Parse("2019-01-02"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 3,  typeId: (int)UserManagementAdministrationActionType.UserCreated,            performed: DateTimeOffset.Parse("2019-01-03"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 4,  typeId: (int)RoleManagementAdministrationActionType.RoleModified,           performed: DateTimeOffset.Parse("2019-01-04"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 5,  typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-01-05"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 6,  typeId: (int)RoleManagementAdministrationActionType.RoleModified,           performed: DateTimeOffset.Parse("2019-01-06"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 7,  typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-01-07"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 8,  typeId: (int)RoleManagementAdministrationActionType.RoleModified,           performed: DateTimeOffset.Parse("2019-01-08"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 9,  typeId: (int)UserManagementAdministrationActionType.UserCreated,            performed: DateTimeOffset.Parse("2019-01-09"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 10, typeId: (int)RoleManagementAdministrationActionType.RoleCreated,            performed: DateTimeOffset.Parse("2019-01-10"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 11, typeId: (int)RoleManagementAdministrationActionType.RoleModified,           performed: DateTimeOffset.Parse("2019-01-11"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 12, typeId: (int)RoleManagementAdministrationActionType.RoleModified,           performed: DateTimeOffset.Parse("2019-01-12"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 13, typeId: (int)RoleManagementAdministrationActionType.RoleModified,           performed: DateTimeOffset.Parse("2019-01-13"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 14, typeId: (int)RoleManagementAdministrationActionType.RoleCreated,            performed: DateTimeOffset.Parse("2019-01-14"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 15, typeId: (int)RoleManagementAdministrationActionType.RoleDeleted,            performed: DateTimeOffset.Parse("2019-01-15"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 16, typeId: (int)RoleManagementAdministrationActionType.RoleDeleted,            performed: DateTimeOffset.Parse("2019-01-16"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 17, typeId: (int)RoleManagementAdministrationActionType.RoleRestored,           performed: DateTimeOffset.Parse("2019-01-17"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 18, typeId: (int)UserManagementAdministrationActionType.DefaultsModified,       performed: DateTimeOffset.Parse("2019-01-18"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 19, typeId: (int)UserManagementAdministrationActionType.DefaultsModified,       performed: DateTimeOffset.Parse("2019-01-19"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 20, typeId: (int)UserManagementAdministrationActionType.DefaultsModified,       performed: DateTimeOffset.Parse("2019-01-20"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 21, typeId: (int)UserManagementAdministrationActionType.DefaultsModified,       performed: DateTimeOffset.Parse("2019-01-21"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 22, typeId: (int)UserManagementAdministrationActionType.DefaultsModified,       performed: DateTimeOffset.Parse("2019-01-22"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 23, typeId: (int)UserManagementAdministrationActionType.DefaultsModified,       performed: DateTimeOffset.Parse("2019-01-23"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 24, typeId: (int)UserManagementAdministrationActionType.DefaultsModified,       performed: DateTimeOffset.Parse("2019-01-24"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 25, typeId: (int)UserManagementAdministrationActionType.DefaultsModified,       performed: DateTimeOffset.Parse("2019-01-25"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 26, typeId: (int)UserManagementAdministrationActionType.DefaultsModified,       performed: DateTimeOffset.Parse("2019-01-26"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 27, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-01-27"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 28, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-01-28"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 29, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-01-29"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 30, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-01-30"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 31, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-01-31"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 32, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-02-01"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 33, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-02-02"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 34, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-02-03"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 35, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-02-04"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 36, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-02-05"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 37, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-02-06"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 38, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-02-07"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 39, typeId: (int)UserManagementAdministrationActionType.UserModified,           performed: DateTimeOffset.Parse("2019-02-08"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 40, typeId: (int)CharacterManagementAdministrationActionType.GuildCreated,      performed: DateTimeOffset.Parse("2019-02-09"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 41, typeId: (int)CharacterManagementAdministrationActionType.GuildModified,     performed: DateTimeOffset.Parse("2019-02-10"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 42, typeId: (int)CharacterManagementAdministrationActionType.GuildCreated,      performed: DateTimeOffset.Parse("2019-02-11"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 43, typeId: (int)CharacterManagementAdministrationActionType.GuildCreated,      performed: DateTimeOffset.Parse("2019-02-12"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 44, typeId: (int)CharacterManagementAdministrationActionType.GuildDeleted,      performed: DateTimeOffset.Parse("2019-02-13"),  performedById: 1    ))
                .Append(new AdministrationActionEntity( id: 45, typeId: (int)CharacterManagementAdministrationActionType.GuildModified,     performed: DateTimeOffset.Parse("2019-02-14"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 46, typeId: (int)CharacterManagementAdministrationActionType.GuildDeleted,      performed: DateTimeOffset.Parse("2019-02-15"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 47, typeId: (int)CharacterManagementAdministrationActionType.GuildRestored,     performed: DateTimeOffset.Parse("2019-02-16"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 48, typeId: (int)CharacterManagementAdministrationActionType.GuildModified,     performed: DateTimeOffset.Parse("2019-02-17"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 49, typeId: (int)CharacterManagementAdministrationActionType.DivisionCreated,   performed: DateTimeOffset.Parse("2019-02-18"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 50, typeId: (int)CharacterManagementAdministrationActionType.DivisionCreated,   performed: DateTimeOffset.Parse("2019-02-19"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 51, typeId: (int)CharacterManagementAdministrationActionType.DivisionCreated,   performed: DateTimeOffset.Parse("2019-02-20"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 52, typeId: (int)CharacterManagementAdministrationActionType.DivisionCreated,   performed: DateTimeOffset.Parse("2019-02-21"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 53, typeId: (int)CharacterManagementAdministrationActionType.DivisionDeleted,   performed: DateTimeOffset.Parse("2019-02-22"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 54, typeId: (int)CharacterManagementAdministrationActionType.DivisionModified,  performed: DateTimeOffset.Parse("2019-02-23"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 55, typeId: (int)CharacterManagementAdministrationActionType.DivisionCreated,   performed: DateTimeOffset.Parse("2019-02-24"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 56, typeId: (int)CharacterManagementAdministrationActionType.DivisionModified,  performed: DateTimeOffset.Parse("2019-02-25"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 57, typeId: (int)CharacterManagementAdministrationActionType.DivisionCreated,   performed: DateTimeOffset.Parse("2019-02-26"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 58, typeId: (int)CharacterManagementAdministrationActionType.DivisionDeleted,   performed: DateTimeOffset.Parse("2019-02-27"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 59, typeId: (int)CharacterManagementAdministrationActionType.DivisionCreated,   performed: DateTimeOffset.Parse("2019-02-28"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 60, typeId: (int)CharacterManagementAdministrationActionType.DivisionCreated,   performed: DateTimeOffset.Parse("2019-03-01"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 61, typeId: (int)CharacterManagementAdministrationActionType.DivisionCreated,   performed: DateTimeOffset.Parse("2019-03-02"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 62, typeId: (int)CharacterManagementAdministrationActionType.DivisionRestored,  performed: DateTimeOffset.Parse("2019-03-03"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 63, typeId: (int)CharacterManagementAdministrationActionType.DivisionModified,  performed: DateTimeOffset.Parse("2019-03-04"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 64, typeId: (int)CharacterManagementAdministrationActionType.DivisionModified,  performed: DateTimeOffset.Parse("2019-03-05"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 65, typeId: (int)CharacterManagementAdministrationActionType.DivisionModified,  performed: DateTimeOffset.Parse("2019-03-06"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 66, typeId: (int)CharacterManagementAdministrationActionType.DivisionModified,  performed: DateTimeOffset.Parse("2019-03-07"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 67, typeId: (int)CharacterManagementAdministrationActionType.CharacterCreated,  performed: DateTimeOffset.Parse("2019-03-07"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 68, typeId: (int)CharacterManagementAdministrationActionType.CharacterCreated,  performed: DateTimeOffset.Parse("2019-03-08"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 69, typeId: (int)CharacterManagementAdministrationActionType.CharacterCreated,  performed: DateTimeOffset.Parse("2019-03-09"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 70, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-10"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 71, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-11"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 72, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-12"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 73, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-13"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 74, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-14"),  performedById: 3    ))
                .Append(new AdministrationActionEntity( id: 75, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-15"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 76, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-16"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 77, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-17"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 78, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-18"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 79, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-19"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 80, typeId: (int)CharacterManagementAdministrationActionType.CharacterModified, performed: DateTimeOffset.Parse("2019-03-20"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 81, typeId: (int)CharacterManagementAdministrationActionType.CharacterDeleted,  performed: DateTimeOffset.Parse("2019-03-21"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 82, typeId: (int)CharacterManagementAdministrationActionType.CharacterCreated,  performed: DateTimeOffset.Parse("2019-03-22"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 83, typeId: (int)CharacterManagementAdministrationActionType.CharacterDeleted,  performed: DateTimeOffset.Parse("2019-03-23"),  performedById: 2    ))
                .Append(new AdministrationActionEntity( id: 84, typeId: (int)CharacterManagementAdministrationActionType.CharacterRestored, performed: DateTimeOffset.Parse("2019-03-24"),  performedById: 2    ))
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


            CharacterGuilds = Enumerable.Empty<CharacterGuildEntity>()
                .Append(new CharacterGuildEntity(   id: 1   ))
                .Append(new CharacterGuildEntity(   id: 2   ))
                .Append(new CharacterGuildEntity(   id: 3   ))
                .Do(cg => cg.Divisions = new List<CharacterGuildDivisionEntity>())
                .ToArray();

            CharacterGuildVersions = Enumerable.Empty<CharacterGuildVersionEntity>()
                .Append(new CharacterGuildVersionEntity(    id: 1,  guildId: 1, name: "Character Guild 1",  isDeleted: false,   creationId: 40, previousVersionId: null,    nextVersionId: 2    ))
                .Append(new CharacterGuildVersionEntity(    id: 2,  guildId: 1, name: "Character Guild 1a", isDeleted: false,   creationId: 41, previousVersionId: 1,       nextVersionId: 9    ))
                .Append(new CharacterGuildVersionEntity(    id: 3,  guildId: 2, name: "Character Guild 2",  isDeleted: false,   creationId: 42, previousVersionId: null,    nextVersionId: 5    ))
                .Append(new CharacterGuildVersionEntity(    id: 4,  guildId: 3, name: "Character Guild 3",  isDeleted: false,   creationId: 43, previousVersionId: null,    nextVersionId: 6    ))
                .Append(new CharacterGuildVersionEntity(    id: 5,  guildId: 2, name: "Character Guild 2",  isDeleted: true,    creationId: 44, previousVersionId: 3,       nextVersionId: null ))
                .Append(new CharacterGuildVersionEntity(    id: 6,  guildId: 3, name: "Character Guild 3a", isDeleted: false,   creationId: 45, previousVersionId: 4,       nextVersionId: 7    ))
                .Append(new CharacterGuildVersionEntity(    id: 7,  guildId: 3, name: "Character Guild 3a", isDeleted: true,    creationId: 46, previousVersionId: 6,       nextVersionId: 8    ))
                .Append(new CharacterGuildVersionEntity(    id: 8,  guildId: 3, name: "Character Guild 3a", isDeleted: false,   creationId: 47, previousVersionId: 7,       nextVersionId: null ))
                .Append(new CharacterGuildVersionEntity(    id: 9,  guildId: 1, name: "Character Guild 1",  isDeleted: false,   creationId: 48, previousVersionId: 2,       nextVersionId: null ))
                .Do(cgv => cgv.Guild = CharacterGuilds.First(cg => cg.Id == cgv.GuildId))
                .Do(cgv => cgv.Creation = AdministrationActions.First(aa => aa.Id == cgv.CreationId))
                .ToArray();
            CharacterGuildVersions
                .Do(cgv => cgv.PreviousVersion = (cgv.PreviousVersionId is null) ? null : CharacterGuildVersions.First(pv => pv.Id == cgv.PreviousVersionId))
                .Do(cgv => cgv.NextVersion = (cgv.NextVersionId is null) ? null : CharacterGuildVersions.First(nv => nv.Id == cgv.NextVersionId))
                .Enumerate();

            CharacterGuildDivisions = Enumerable.Empty<CharacterGuildDivisionEntity>()
                .Append(new CharacterGuildDivisionEntity(   id: 1,  guildId: 1   ))
                .Append(new CharacterGuildDivisionEntity(   id: 2,  guildId: 2   ))
                .Append(new CharacterGuildDivisionEntity(   id: 3,  guildId: 2   ))
                .Append(new CharacterGuildDivisionEntity(   id: 4,  guildId: 1   ))
                .Append(new CharacterGuildDivisionEntity(   id: 5,  guildId: 3   ))
                .Append(new CharacterGuildDivisionEntity(   id: 6,  guildId: 2   ))
                .Append(new CharacterGuildDivisionEntity(   id: 7,  guildId: 3   ))
                .Append(new CharacterGuildDivisionEntity(   id: 8,  guildId: 1   ))
                .Append(new CharacterGuildDivisionEntity(   id: 9,  guildId: 3   ))
                .Do(cgd => cgd.Guild = CharacterGuilds.First(cg => cg.Id == cgd.GuildId))
                .Do(cgd => cgd.Versions = new List<CharacterGuildDivisionVersionEntity>())
                .Do(cgd => cgd.Guild.Divisions.Add(cgd))
                .ToArray();

            CharacterGuildDivisionVersions = Enumerable.Empty<CharacterGuildDivisionVersionEntity>()
                .Append(new CharacterGuildDivisionVersionEntity(    id: 1,  divisionId: 1,  name: "Character Guild 1, Division 1",  isDeleted: false,   creationId: 49, previousVersionId: null, nextVersionId: 6       ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 2,  divisionId: 2,  name: "Character Guild 2, Division 1",  isDeleted: false,   creationId: 50, previousVersionId: null, nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 3,  divisionId: 3,  name: "Character Guild 2, Division 2",  isDeleted: false,   creationId: 51, previousVersionId: null, nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 4,  divisionId: 4,  name: "Character Guild 1, Division 2",  isDeleted: false,   creationId: 52, previousVersionId: null, nextVersionId: 5       ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 5,  divisionId: 4,  name: "Character Guild 1, Division 2",  isDeleted: true,    creationId: 53, previousVersionId: 4,    nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 6,  divisionId: 1,  name: "Character Guild 1, Division 1a", isDeleted: false,   creationId: 54, previousVersionId: 1,    nextVersionId: 10      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 7,  divisionId: 5,  name: "Character Guild 3, Division 1",  isDeleted: false,   creationId: 55, previousVersionId: null, nextVersionId: 8       ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 8,  divisionId: 5,  name: "Character Guild 3, Division 1a", isDeleted: false,   creationId: 56, previousVersionId: 7,    nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 9,  divisionId: 6,  name: "Character Guild 2, Division 3",  isDeleted: false,   creationId: 57, previousVersionId: null, nextVersionId: 17      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 10, divisionId: 1,  name: "Character Guild 1, Division 1a", isDeleted: true,    creationId: 58, previousVersionId: 6,    nextVersionId: 14      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 11, divisionId: 7,  name: "Character Guild 3, Division 2",  isDeleted: false,   creationId: 59, previousVersionId: null, nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 12, divisionId: 8,  name: "Character Guild 1, Division 3",  isDeleted: false,   creationId: 60, previousVersionId: null, nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 13, divisionId: 9,  name: "Character Guild 3, Division 3",  isDeleted: false,   creationId: 61, previousVersionId: null, nextVersionId: 15      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 14, divisionId: 1,  name: "Character Guild 1, Division 1a", isDeleted: false,   creationId: 62, previousVersionId: 10,   nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 15, divisionId: 9,  name: "Character Guild 3, Division 3a", isDeleted: false,   creationId: 63, previousVersionId: 13,   nextVersionId: 16      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 16, divisionId: 9,  name: "Character Guild 3, Division 3b", isDeleted: false,   creationId: 64, previousVersionId: 15,   nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 17, divisionId: 6,  name: "Character Guild 2, Division 3a", isDeleted: false,   creationId: 65, previousVersionId: 9,    nextVersionId: 18      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 18, divisionId: 6,  name: "Character Guild 2, Division 3",  isDeleted: false,   creationId: 66, previousVersionId: 17,   nextVersionId: null    ))
                .Do(cgdv => cgdv.Division = CharacterGuildDivisions.First(cgd => cgd.Id == cgdv.DivisionId))
                .Do(cgdv => cgdv.Creation = AdministrationActions.First(aa => aa.Id == cgdv.CreationId))
                .Do(cgdv => cgdv.Division.Versions.Add(cgdv))
                .ToArray();
            CharacterGuildDivisionVersions
                .Do(cgdv => cgdv.PreviousVersion = (cgdv.PreviousVersionId is null) ? null : CharacterGuildDivisionVersions.First(pv => pv.Id == cgdv.PreviousVersionId))
                .Do(cgdv => cgdv.NextVersion = (cgdv.NextVersionId is null) ? null : CharacterGuildDivisionVersions.First(nv => nv.Id == cgdv.NextVersionId))
                .Enumerate();

            CharacterLevelDefinitions = Enumerable.Empty<CharacterLevelDefinitionEntity>()
                .Append(new CharacterLevelDefinitionEntity( level: 1    ))
                .Append(new CharacterLevelDefinitionEntity( level: 2    ))
                .Append(new CharacterLevelDefinitionEntity( level: 3    ))
                .ToArray();

            CharacterLevelDefinitionVersions = Enumerable.Empty<CharacterLevelDefinitionVersionEntity>()
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 1L,     level: 1,   experienceThreshold: 10M,   isDeleted: false,   creationId: 50L,    previousVersionId: null,    nextVersionId: 4L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 2L,     level: 2,   experienceThreshold: 20M,   isDeleted: false,   creationId: 50L,    previousVersionId: null,    nextVersionId: null ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 3L,     level: 3,   experienceThreshold: 30M,   isDeleted: false,   creationId: 50L,    previousVersionId: null,    nextVersionId: 6L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 4L,     level: 1,   experienceThreshold: 11M,   isDeleted: false,   creationId: 50L,    previousVersionId: 1L,      nextVersionId: 5L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 5L,     level: 1,   experienceThreshold: 11M,   isDeleted: true,    creationId: 50L,    previousVersionId: 4L,      nextVersionId: 8L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 6L,     level: 3,   experienceThreshold: 31M,   isDeleted: false,   creationId: 50L,    previousVersionId: 3L,      nextVersionId: 7L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 7L,     level: 3,   experienceThreshold: 31M,   isDeleted: true,    creationId: 50L,    previousVersionId: 6L,      nextVersionId: null ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 8L,     level: 1,   experienceThreshold: 11M,   isDeleted: false,   creationId: 50L,    previousVersionId: 5L,      nextVersionId: 9L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 9L,     level: 1,   experienceThreshold: 12M,   isDeleted: false,   creationId: 50L,    previousVersionId: 8L,      nextVersionId: null ))
                .Do(cldv => cldv.Definition = CharacterLevelDefinitions.First(cld => cld.Level == cldv.Level))
                .Do(cldv => cldv.Creation = AdministrationActions.First(aa => aa.Id == cldv.CreationId))
                .ToArray();
            CharacterLevelDefinitionVersions
                .Do(cldv => cldv.PreviousVersion = (cldv.PreviousVersionId is null) ? null : CharacterLevelDefinitionVersions.First(pv => pv.Id == cldv.PreviousVersionId))
                .Do(cldv => cldv.NextVersion = (cldv.NextVersionId is null) ? null : CharacterLevelDefinitionVersions.First(pv => pv.Id == cldv.NextVersionId))
                .Enumerate();

            Characters = Enumerable.Empty<CharacterEntity>()
                .Append(new CharacterEntity(    id: 1,  ownerId: 1  ))
                .Append(new CharacterEntity(    id: 2,  ownerId: 2  ))
                .Append(new CharacterEntity(    id: 3,  ownerId: 3  ))
                .Append(new CharacterEntity(    id: 4,  ownerId: 1  ))
                .Do(c => c.Owner = Users.First(u => u.Id == c.OwnerId))
                .ToArray();

            CharacterVersions = Enumerable.Empty<CharacterVersionEntity>()
                .Append(new CharacterVersionEntity( id: 1,  characterId: 1, name: "Character 1",    divisionId: 1,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 67, previousVersionId: null,    nextVersionId: 12   ))
                .Append(new CharacterVersionEntity( id: 2,  characterId: 2, name: "Character 2",    divisionId: 3,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 68, previousVersionId: null,    nextVersionId: 9    ))
                .Append(new CharacterVersionEntity( id: 3,  characterId: 3, name: "Character 3",    divisionId: 3,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 69, previousVersionId: null,    nextVersionId: 4    ))
                .Append(new CharacterVersionEntity( id: 4,  characterId: 3, name: "Character 3a",   divisionId: 1,  experiencePoints: 100,  goldAmount: 900,    insanityValue: 9,     isDeleted: false,   creationId: 70, previousVersionId: 3,       nextVersionId: 5    ))
                .Append(new CharacterVersionEntity( id: 5,  characterId: 3, name: "Character 3a",   divisionId: 1,  experiencePoints: 200,  goldAmount: 850,    insanityValue: 9,     isDeleted: false,   creationId: 71, previousVersionId: 4,       nextVersionId: 6    ))
                .Append(new CharacterVersionEntity( id: 6,  characterId: 3, name: "Character 3a",   divisionId: 2,  experiencePoints: 200,  goldAmount: 850,    insanityValue: 9,     isDeleted: false,   creationId: 72, previousVersionId: 5,       nextVersionId: 7    ))
                .Append(new CharacterVersionEntity( id: 7,  characterId: 3, name: "Character 3b",   divisionId: 2,  experiencePoints: 200,  goldAmount: 850,    insanityValue: 9,     isDeleted: false,   creationId: 73, previousVersionId: 6,       nextVersionId: 8    ))
                .Append(new CharacterVersionEntity( id: 8,  characterId: 3, name: "Character 3b",   divisionId: 2,  experiencePoints: 250,  goldAmount: 750,    insanityValue: 10,    isDeleted: false,   creationId: 74, previousVersionId: 7,       nextVersionId: 14   ))
                .Append(new CharacterVersionEntity( id: 9,  characterId: 2, name: "Character 2",    divisionId: 3,  experiencePoints: 100,  goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 75, previousVersionId: 2,       nextVersionId: 10   ))
                .Append(new CharacterVersionEntity( id: 10, characterId: 2, name: "Character 2",    divisionId: 3,  experiencePoints: 250,  goldAmount: 400,    insanityValue: 10,    isDeleted: false,   creationId: 76, previousVersionId: 9,       nextVersionId: 11   ))
                .Append(new CharacterVersionEntity( id: 11, characterId: 2, name: "Character 2a",   divisionId: 3,  experiencePoints: 250,  goldAmount: 400,    insanityValue: 10,    isDeleted: false,   creationId: 77, previousVersionId: 10,      nextVersionId: 13   ))
                .Append(new CharacterVersionEntity( id: 12, characterId: 1, name: "Character 1",    divisionId: 1,  experiencePoints: 300,  goldAmount: 1200,   insanityValue: 10,    isDeleted: false,   creationId: 78, previousVersionId: 1,       nextVersionId: 15   ))
                .Append(new CharacterVersionEntity( id: 13, characterId: 2, name: "Character 2a",   divisionId: 3,  experiencePoints: 550,  goldAmount: 600,    insanityValue: 10,    isDeleted: false,   creationId: 79, previousVersionId: 11,      nextVersionId: null ))
                .Append(new CharacterVersionEntity( id: 14, characterId: 3, name: "Character 3b",   divisionId: 2,  experiencePoints: 550,  goldAmount: 950,    insanityValue: 10,    isDeleted: false,   creationId: 80, previousVersionId: 8,       nextVersionId: null ))
                .Append(new CharacterVersionEntity( id: 15, characterId: 1, name: "Character 1",    divisionId: 1,  experiencePoints: 300,  goldAmount: 1200,   insanityValue: 10,    isDeleted: true,    creationId: 81, previousVersionId: 12,      nextVersionId: null ))
                .Append(new CharacterVersionEntity( id: 16, characterId: 4, name: "Character 4",    divisionId: 1,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 82, previousVersionId: null,    nextVersionId: 17   ))
                .Append(new CharacterVersionEntity( id: 17, characterId: 4, name: "Character 4",    divisionId: 1,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: true,    creationId: 83, previousVersionId: 16,      nextVersionId: 18   ))
                .Append(new CharacterVersionEntity( id: 18, characterId: 4, name: "Character 4",    divisionId: 1,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 84, previousVersionId: 17,      nextVersionId: null ))
                .Do(cv => cv.Character = Characters.First(c => c.Id == cv.CharacterId))
                .Do(cv => cv.Creation = AdministrationActions.First(aa => aa.Id == cv.CreationId))
                .ToArray();
            CharacterVersions
                .Do(cv => cv.PreviousVersion = (cv.PreviousVersionId is null) ? null : CharacterVersions.First(pv => pv.Id == cv.PreviousVersionId))
                .Do(cv => cv.NextVersion = (cv.NextVersionId is null) ? null : CharacterVersions.First(nv => nv.Id == cv.NextVersionId))
                .Enumerate();


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

        #region Characters

        public readonly IReadOnlyList<CharacterGuildEntity> CharacterGuilds;

        public readonly IReadOnlyList<CharacterGuildDivisionEntity> CharacterGuildDivisions;

        public readonly IReadOnlyList<CharacterGuildDivisionVersionEntity> CharacterGuildDivisionVersions;

        public readonly IReadOnlyList<CharacterGuildVersionEntity> CharacterGuildVersions;

        public readonly IReadOnlyList<CharacterLevelDefinitionEntity> CharacterLevelDefinitions;

        public readonly IReadOnlyList<CharacterLevelDefinitionVersionEntity> CharacterLevelDefinitionVersions;

        public readonly IReadOnlyList<CharacterVersionEntity> CharacterVersions;
        
        public readonly IReadOnlyList<CharacterEntity> Characters;

        #endregion Characters

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
