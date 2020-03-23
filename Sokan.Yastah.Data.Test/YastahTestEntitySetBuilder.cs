using System;
using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Auditing;
using Sokan.Yastah.Data.Authentication;
using Sokan.Yastah.Data.Characters;
using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Roles;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test
{
    internal abstract class YastahTestEntitySetBuilder
    {
        #region Public Methods

        public YastahTestEntitySet Build()
        {
            var administrationActionCategories = CreateAdministrationActionCategories();
            var administrationActionTypes = CreateAdministrationActionTypes();
            var administrationActions = CreateAdministrationActions();
            var authenticationTickets = CreateAuthenticationTickets();
            var characterGuilds = CreateCharacterGuilds();
            var characterGuildDivisions = CreateCharacterGuildDivisions();
            var characterGuildDivisionVersions = CreateCharacterGuildDivisionVersions();
            var characterGuildVersions = CreateCharacterGuildVersions();
            var characterLevelDefinitions = CreateCharacterLevelDefinitions();
            var characterLevelDefinitionVersions = CreateCharacterLevelDefinitionVersions();
            var characterVersions = CreateCharacterVersions();
            var characters = CreateCharacters();
            var permissionCategories = CreatePermissionCategories();
            var permissions = CreatePermissions();
            var rolePermissionMappings = CreateRolePermissionMappings();
            var roleVersions = CreateRoleVersions();
            var roles = CreateRoles();
            var defaultPermissionMappings = CreateDefaultPermissionMappings();
            var defaultRoleMappings = CreateDefaultRoleMappings();
            var userPermissionMappings = CreateUserPermissionMappings();
            var userRoleMappings = CreateUserRoleMappings();
            var users = CreateUsers();


            // I'm really not a fan of this method of building navigations, for testing. But the only other feasible way to do it
            // is by using Microsoft.EntityFrameworkCore.InMemory, and last time I tried it, a full test run (819 tests) went from 8 seconds to 52 seconds.
            // No.
            if ((administrationActionTypes is { })
                    && (administrationActionCategories is { }))
                foreach (var administrationActionType in administrationActionTypes)
                    administrationActionType.Category = administrationActionCategories
                        .First(administrationActionCategory => administrationActionCategory.Id == administrationActionType.CategoryId);

            if ((administrationActions is { })
                    && ((administrationActionTypes is { }) || (administrationActions is { })))
                foreach (var administrationAction in administrationActions)
                {
                    if (administrationActionTypes is { })
                        administrationAction.Type = administrationActionTypes
                            .First(administrationActionType => administrationActionType.Id == administrationAction.TypeId);

                    if (administrationActions is { })
                        if ((administrationAction.PerformedById is { }) && (users is { }))
                            administrationAction.PerformedBy = users
                                .First(user => user.Id == administrationAction.PerformedById);
                }

            if ((authenticationTickets is { })
                    && ((users is { }) || (administrationActions is { })))
                foreach (var authenticationTicket in authenticationTickets)
                {
                    if (users is { })
                        authenticationTicket.User = users
                            .First(user => user.Id == authenticationTicket.UserId);

                    if (administrationActions is { })
                    {
                        authenticationTicket.Creation = administrationActions
                            .First(administrationAction => administrationAction.Id == authenticationTicket.CreationId);

                        if (authenticationTicket.DeletionId is { })
                            authenticationTicket.Deletion = administrationActions
                                .First(administrationAction => administrationAction.Id == authenticationTicket.DeletionId);
                    }
                }

            if ((characterGuilds is { })
                    && (characterGuildDivisions is { }))
                foreach (var characterGuild in characterGuilds)
                    characterGuild.Divisions = characterGuildDivisions
                        .Where(characterGuildDivision => characterGuildDivision.GuildId == characterGuild.Id)
                        .ToList();

            if ((characterGuildDivisions is { })
                    && ((characterGuilds is { }) || (characterGuildDivisionVersions is { })))
                foreach (var characterGuildDivision in characterGuildDivisions)
                {
                    if (characterGuilds is { })
                        characterGuildDivision.Guild = characterGuilds
                            .First(characterGuild => characterGuild.Id == characterGuildDivision.GuildId);

                    if (characterGuildDivisionVersions is { })
                        characterGuildDivision.Versions = characterGuildDivisionVersions
                            .Where(characterGuildDivisionVersion => characterGuildDivisionVersion.DivisionId == characterGuildDivision.Id)
                            .ToList();
                }

            if (characterGuildDivisionVersions is { })
                foreach (var characterGuildDivisionVersion in characterGuildDivisionVersions)
                {
                    if (characterGuildDivisions is { })
                        characterGuildDivisionVersion.Division = characterGuildDivisions
                            .First(characterGuildDivision => characterGuildDivision.Id == characterGuildDivisionVersion.DivisionId);

                    if (administrationActions is { })
                        characterGuildDivisionVersion.Creation = administrationActions
                            .First(administrationAction => administrationAction.Id == characterGuildDivisionVersion.CreationId);

                    if (characterGuildDivisionVersion.PreviousVersionId is { })
                        characterGuildDivisionVersion.PreviousVersion = characterGuildDivisionVersions
                            .First(previousVersion => previousVersion.Id == characterGuildDivisionVersion.PreviousVersionId);

                    if (characterGuildDivisionVersion.NextVersionId is { })
                        characterGuildDivisionVersion.NextVersion = characterGuildDivisionVersions
                            .First(nextVersion => nextVersion.Id == characterGuildDivisionVersion.NextVersionId);
                }


            if (characterGuildVersions is { })
                foreach (var characterGuildVersion in characterGuildVersions)
                {
                    if (characterGuilds is { })
                        characterGuildVersion.Guild = characterGuilds
                            .First(characterGuild => characterGuild.Id == characterGuildVersion.GuildId);

                    if (administrationActions is { })
                        characterGuildVersion.Creation = administrationActions
                        .First(administrationAction => administrationAction.Id == characterGuildVersion.CreationId);

                    if (characterGuildVersion.PreviousVersionId is { })
                        characterGuildVersion.PreviousVersion = characterGuildVersions
                            .First(previousVersion => previousVersion.Id == characterGuildVersion.PreviousVersionId);

                    if (characterGuildVersion.NextVersionId is { })
                        characterGuildVersion.NextVersion = characterGuildVersions
                            .First(nextVersion => nextVersion.Id == characterGuildVersion.NextVersionId);
                }

            if (characterLevelDefinitionVersions is { })
                foreach (var characterLevelDefinitionVersion in characterLevelDefinitionVersions)
                {
                    if (characterLevelDefinitions is { })
                        characterLevelDefinitionVersion.Definition = characterLevelDefinitions
                            .First(characterLevelDefinition => characterLevelDefinition.Level == characterLevelDefinitionVersion.Level);

                    if (administrationActions is { })
                        characterLevelDefinitionVersion.Creation = administrationActions
                            .First(administrationAction => administrationAction.Id == characterLevelDefinitionVersion.CreationId);

                    if (characterLevelDefinitionVersion.PreviousVersionId is { })
                        characterLevelDefinitionVersion.PreviousVersion = characterLevelDefinitionVersions
                            .First(previousVersion => previousVersion.Id == characterLevelDefinitionVersion.PreviousVersionId);

                    if (characterLevelDefinitionVersion.NextVersionId is { })
                        characterLevelDefinitionVersion.NextVersion = characterLevelDefinitionVersions
                            .First(nextVersion => nextVersion.Id == characterLevelDefinitionVersion.NextVersionId);
                }

            if (characterVersions is { })
                foreach (var characterVersion in characterVersions)
                {
                    if (characters is { })
                        characterVersion.Character = characters
                            .First(character => character.Id == characterVersion.CharacterId);

                    if (characterGuildDivisions is { })
                        characterVersion.Division = characterGuildDivisions
                            .First(characterGuildDivision => characterGuildDivision.Id == characterVersion.DivisionId);

                    if (administrationActions is { })
                        characterVersion.Creation = administrationActions
                            .First(administrationAction => administrationAction.Id == characterVersion.CreationId);

                    if (characterVersion.PreviousVersionId is { })
                        characterVersion.PreviousVersion = characterVersions
                            .First(previousVersion => previousVersion.Id == characterVersion.PreviousVersionId);
                    if (characterVersion.NextVersionId is { })
                        characterVersion.NextVersion = characterVersions
                            .First(nextVersion => nextVersion.Id == characterVersion.NextVersionId);
                }

            if ((characters is { })
                    && (users is { }))
                foreach (var character in characters)
                    character.Owner = users
                        .First(user => user.Id == character.OwnerId);

            if ((permissionCategories is { })
                    && (permissions is { }))
                foreach (var permissionCategory in permissionCategories)
                    permissionCategory.Permissions = permissions
                        .Where(permission => permission.CategoryId == permissionCategory.Id)
                        .ToList();

            if ((permissions is { })
                    && (permissionCategories is { }))
                foreach (var permission in permissions)
                    permission.Category = permissionCategories
                        .First(permissionCategory => permissionCategory.Id == permission.CategoryId);

            if ((rolePermissionMappings is { })
                    && ((roles is { }) || (permissions is { }) || (administrationActions is { })))
                foreach (var rolePermissionMapping in rolePermissionMappings)
                {
                    if (roles is { })
                        rolePermissionMapping.Role = roles
                            .First(role => role.Id == rolePermissionMapping.RoleId);

                    if (permissions is { })
                        rolePermissionMapping.Permission = permissions
                            .First(permission => permission.PermissionId == rolePermissionMapping.PermissionId);

                    if (administrationActions is { })
                    {
                        rolePermissionMapping.Creation = administrationActions
                            .First(administrationAction => administrationAction.Id == rolePermissionMapping.CreationId);

                        if (rolePermissionMapping.DeletionId is { })
                            rolePermissionMapping.Deletion = administrationActions
                                .First(administrationAction => administrationAction.Id == rolePermissionMapping.DeletionId);
                    }
                }

            if (roleVersions is { })
                foreach (var roleVersion in roleVersions)
                {
                    if (roles is { })
                        roleVersion.Role = roles
                            .First(role => role.Id == roleVersion.RoleId);

                    if (administrationActions is { })
                        roleVersion.Creation = administrationActions
                            .First(administrationAction => administrationAction.Id == roleVersion.CreationId);

                    if (roleVersion.PreviousVersionId is { })
                        roleVersion.PreviousVersion = roleVersions
                            .First(previousVersion => previousVersion.Id == roleVersion.PreviousVersionId);

                    if (roleVersion.NextVersionId is { })
                        roleVersion.NextVersion = roleVersions
                            .First(nextVersion => nextVersion.Id == roleVersion.NextVersionId);
                }

            if ((roles is { })
                    && (rolePermissionMappings is { }))
                foreach (var role in roles)
                    role.PermissionMappings = rolePermissionMappings
                        .Where(rolePermissionMapping => rolePermissionMapping.RoleId == role.Id)
                        .ToList();

            if ((defaultPermissionMappings is { })
                    && ((permissions is { }) || (administrationActions is { })))
                foreach(var defaultPermissionMapping in defaultPermissionMappings)
                {
                    if(permissions is { })
                        defaultPermissionMapping.Permission = permissions
                            .First(permission => permission.PermissionId == defaultPermissionMapping.PermissionId);
                    
                    if(administrationActions is { })
                    {
                        defaultPermissionMapping.Creation = administrationActions
                            .First(administrationAction => administrationAction.Id == defaultPermissionMapping.CreationId);

                        if (defaultPermissionMapping.DeletionId is { })
                            defaultPermissionMapping.Deletion = administrationActions
                                .First(administrationAction => administrationAction.Id == defaultPermissionMapping.DeletionId);
                    }
                }

            if ((defaultRoleMappings is { })
                    && ((roles is { }) || (administrationActions is { })))
                foreach(var defaultRoleMapping in defaultRoleMappings)
                {
                    if (roles is { })
                        defaultRoleMapping.Role = roles
                            .First(role => role.Id == defaultRoleMapping.RoleId);

                    if (administrationActions is { })
                    {
                        defaultRoleMapping.Creation = administrationActions
                            .First(administrationAction => administrationAction.Id == defaultRoleMapping.CreationId);

                        if (defaultRoleMapping.DeletionId is { })
                            defaultRoleMapping.Deletion = administrationActions
                                .First(administrationAction => administrationAction.Id == defaultRoleMapping.DeletionId);
                    }
                }

            if ((userPermissionMappings is { })
                    && ((users is { }) || (permissions is { }) || (administrationActions is { })))
                foreach(var userPermissionMapping in userPermissionMappings)
                {
                    if(users is { })
                        userPermissionMapping.User = users
                            .First(user => user.Id == userPermissionMapping.UserId);
                    
                    if (permissions is { })
                        userPermissionMapping.Permission = permissions
                            .First(permission => permission.PermissionId == userPermissionMapping.PermissionId);
                    
                    if (administrationActions is { })
                    {
                        userPermissionMapping.Creation = administrationActions
                            .First(administrationAction => administrationAction.Id == userPermissionMapping.CreationId);

                        if (userPermissionMapping.DeletionId is { })
                            userPermissionMapping.Deletion = administrationActions
                                .First(administrationAction => administrationAction.Id == userPermissionMapping.DeletionId);
                    }
                }

            if ((userRoleMappings is { })
                    && ((users is { }) || (roles is { }) || (administrationActions is { })))
                foreach(var userRoleMapping in userRoleMappings)
                {
                    if (users is { })
                        userRoleMapping.User = users
                            .First(user => user.Id == userRoleMapping.UserId);
                    
                    if (roles is { })
                        userRoleMapping.Role = roles
                            .First(role => role.Id == userRoleMapping.RoleId);
                    
                    if (administrationActions is { })
                    {
                        userRoleMapping.Creation = administrationActions
                            .First(administrationAction => administrationAction.Id == userRoleMapping.CreationId);

                        if (userRoleMapping.DeletionId is { })
                            userRoleMapping.Deletion = administrationActions
                                .First(administrationAction => administrationAction.Id == userRoleMapping.DeletionId);
                    }
                }

            if ((users is { })
                    && ((userPermissionMappings is { }) || (userRoleMappings is { })))
                foreach(var user in users)
                {
                    if(userPermissionMappings is { })
                        user.PermissionMappings = userPermissionMappings
                            .Where(userPermissionMapping => userPermissionMapping.UserId == user.Id)
                            .ToList();

                    if (userRoleMappings is { })
                        user.RoleMappings = userRoleMappings
                            .Where(userRoleMapping => userRoleMapping.UserId == user.Id)
                            .ToList();
                }

            return new YastahTestEntitySet(
                administrationActionCategories      ?? Array.Empty<AuditableActionCategoryEntity>(),
                administrationActionTypes           ?? Array.Empty<AuditableActionTypeEntity>(),
                administrationActions               ?? Array.Empty<AuditableActionEntity>(),
                authenticationTickets               ?? Array.Empty<AuthenticationTicketEntity>(),
                characterGuilds                     ?? Array.Empty<CharacterGuildEntity>(),
                characterGuildDivisions             ?? Array.Empty<CharacterGuildDivisionEntity>(),
                characterGuildDivisionVersions      ?? Array.Empty<CharacterGuildDivisionVersionEntity>(),
                characterGuildVersions              ?? Array.Empty<CharacterGuildVersionEntity>(),
                characterLevelDefinitions           ?? Array.Empty<CharacterLevelDefinitionEntity>(),
                characterLevelDefinitionVersions    ?? Array.Empty<CharacterLevelDefinitionVersionEntity>(),
                characterVersions                   ?? Array.Empty<CharacterVersionEntity>(),
                characters                          ?? Array.Empty<CharacterEntity>(),
                permissionCategories                ?? Array.Empty<PermissionCategoryEntity>(),
                permissions                         ?? Array.Empty<PermissionEntity>(),
                rolePermissionMappings              ?? Array.Empty<RolePermissionMappingEntity>(),
                roleVersions                        ?? Array.Empty<RoleVersionEntity>(),
                roles                               ?? Array.Empty<RoleEntity>(),
                defaultPermissionMappings           ?? Array.Empty<DefaultPermissionMappingEntity>(),
                defaultRoleMappings                 ?? Array.Empty<DefaultRoleMappingEntity>(),
                userPermissionMappings              ?? Array.Empty<UserPermissionMappingEntity>(),
                userRoleMappings                    ?? Array.Empty<UserRoleMappingEntity>(),
                users                               ?? Array.Empty<UserEntity>());
        }

        #endregion Public Methods

        #region Administration

        protected virtual IReadOnlyList<AuditableActionCategoryEntity>? CreateAdministrationActionCategories()
            => null;

        protected virtual IReadOnlyList<AuditableActionTypeEntity>? CreateAdministrationActionTypes()
            => null;

        protected virtual IReadOnlyList<AuditableActionEntity>? CreateAdministrationActions()
            => null;

        #endregion Administration

        #region Authentication

        protected virtual IReadOnlyList<AuthenticationTicketEntity>? CreateAuthenticationTickets()
            => null;

        #endregion Authentication

        #region Characters

        protected virtual IReadOnlyList<CharacterGuildEntity>? CreateCharacterGuilds()
            => null;

        protected virtual IReadOnlyList<CharacterGuildDivisionEntity>? CreateCharacterGuildDivisions()
            => null;

        protected virtual IReadOnlyList<CharacterGuildDivisionVersionEntity>? CreateCharacterGuildDivisionVersions()
            => null;

        protected virtual IReadOnlyList<CharacterGuildVersionEntity>? CreateCharacterGuildVersions()
            => null;

        protected virtual IReadOnlyList<CharacterLevelDefinitionEntity>? CreateCharacterLevelDefinitions()
            => null;

        protected virtual IReadOnlyList<CharacterLevelDefinitionVersionEntity>? CreateCharacterLevelDefinitionVersions()
            => null;

        protected virtual IReadOnlyList<CharacterVersionEntity>? CreateCharacterVersions()
            => null;

        protected virtual IReadOnlyList<CharacterEntity>? CreateCharacters()
            => null;

        #endregion Characters

        #region Permission

        protected virtual IReadOnlyList<PermissionCategoryEntity>? CreatePermissionCategories()
            => null;

        protected virtual IReadOnlyList<PermissionEntity>? CreatePermissions()
            => null;

        #endregion Permission

        #region Roles

        protected virtual IReadOnlyList<RolePermissionMappingEntity>? CreateRolePermissionMappings()
            => null;

        protected virtual IReadOnlyList<RoleVersionEntity>? CreateRoleVersions()
            => null;

        protected virtual IReadOnlyList<RoleEntity>? CreateRoles()
            => null;

        #endregion Roles

        #region Users

        protected virtual IReadOnlyList<DefaultPermissionMappingEntity>? CreateDefaultPermissionMappings()
            => null;

        protected virtual IReadOnlyList<DefaultRoleMappingEntity>? CreateDefaultRoleMappings()
            => null;

        protected virtual IReadOnlyList<UserPermissionMappingEntity>? CreateUserPermissionMappings()
            => null;

        protected virtual IReadOnlyList<UserRoleMappingEntity>? CreateUserRoleMappings()
            => null;

        protected virtual IReadOnlyList<UserEntity>? CreateUsers()
            => null;

        #endregion Users
    }
}
