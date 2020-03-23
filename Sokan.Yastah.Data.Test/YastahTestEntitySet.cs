using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore.Storage;

using Sokan.Yastah.Data.Auditing;
using Sokan.Yastah.Data.Authentication;
using Sokan.Yastah.Data.Characters;
using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Roles;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test
{
    internal class YastahTestEntitySet
    {
        #region Construction

        public YastahTestEntitySet(
            IReadOnlyList<AuditableActionCategoryEntity> administrationActionCategories,
            IReadOnlyList<AuditableActionTypeEntity> administrationActionTypes,
            IReadOnlyList<AuditableActionEntity> administrationActions,
            IReadOnlyList<AuthenticationTicketEntity> authenticationTickets,
            IReadOnlyList<CharacterGuildEntity> characterGuilds,
            IReadOnlyList<CharacterGuildDivisionEntity> characterGuildDivisions,
            IReadOnlyList<CharacterGuildDivisionVersionEntity> characterGuildDivisionVersions,
            IReadOnlyList<CharacterGuildVersionEntity> characterGuildVersions,
            IReadOnlyList<CharacterLevelDefinitionEntity> characterLevelDefinitions,
            IReadOnlyList<CharacterLevelDefinitionVersionEntity> characterLevelDefinitionVersions,
            IReadOnlyList<CharacterVersionEntity> characterVersions,
            IReadOnlyList<CharacterEntity> characters,
            IReadOnlyList<PermissionCategoryEntity> permissionCategories,
            IReadOnlyList<PermissionEntity> permissions,
            IReadOnlyList<RolePermissionMappingEntity> rolePermissionMappings,
            IReadOnlyList<RoleVersionEntity> roleVersions,
            IReadOnlyList<RoleEntity> roles,
            IReadOnlyList<DefaultPermissionMappingEntity> defaultPermissionMappings,
            IReadOnlyList<DefaultRoleMappingEntity> defaultRoleMappings,
            IReadOnlyList<UserPermissionMappingEntity> userPermissionMappings,
            IReadOnlyList<UserRoleMappingEntity> userRoleMappings,
            IReadOnlyList<UserEntity> users)
        {
            AdministrationActionCategories = administrationActionCategories;
            AdministrationActionTypes = administrationActionTypes;
            AdministrationActions = administrationActions;
            AuthenticationTickets = authenticationTickets;
            CharacterGuilds = characterGuilds;
            CharacterGuildDivisions = characterGuildDivisions;
            CharacterGuildDivisionVersions = characterGuildDivisionVersions;
            CharacterGuildVersions = characterGuildVersions;
            CharacterLevelDefinitions = characterLevelDefinitions;
            CharacterLevelDefinitionVersions = characterLevelDefinitionVersions;
            CharacterVersions = characterVersions;
            Characters = characters;
            PermissionCategories = permissionCategories;
            Permissions = permissions;
            RolePermissionMappings = rolePermissionMappings;
            RoleVersions = roleVersions;
            Roles = roles;
            DefaultPermissionMappings = defaultPermissionMappings;
            DefaultRoleMappings = defaultRoleMappings;
            UserPermissionMappings = userPermissionMappings;
            UserRoleMappings = userRoleMappings;
            Users = users;
        }

        #endregion Construction

        #region Administration

        public IReadOnlyList<AuditableActionCategoryEntity> AdministrationActionCategories { get; }

        public IReadOnlyList<AuditableActionTypeEntity> AdministrationActionTypes { get; }

        public IReadOnlyList<AuditableActionEntity> AdministrationActions { get; }

        #endregion Administration

        #region Authentication

        public IReadOnlyList<AuthenticationTicketEntity> AuthenticationTickets { get; }

        #endregion Authentication

        #region Characters

        public IReadOnlyList<CharacterGuildEntity> CharacterGuilds { get; }

        public IReadOnlyList<CharacterGuildDivisionEntity> CharacterGuildDivisions { get; }

        public IReadOnlyList<CharacterGuildDivisionVersionEntity> CharacterGuildDivisionVersions { get; }

        public IReadOnlyList<CharacterGuildVersionEntity> CharacterGuildVersions { get; }

        public IReadOnlyList<CharacterLevelDefinitionEntity> CharacterLevelDefinitions { get; }

        public IReadOnlyList<CharacterLevelDefinitionVersionEntity> CharacterLevelDefinitionVersions { get; }

        public IReadOnlyList<CharacterVersionEntity> CharacterVersions { get; }

        public IReadOnlyList<CharacterEntity> Characters { get; }

        #endregion Characters

        #region Permission

        public IReadOnlyList<PermissionCategoryEntity> PermissionCategories { get; }

        public IReadOnlyList<PermissionEntity> Permissions { get; }

        #endregion Permission

        #region Roles

        public IReadOnlyList<RolePermissionMappingEntity> RolePermissionMappings { get; }

        public IReadOnlyList<RoleVersionEntity> RoleVersions { get; }

        public IReadOnlyList<RoleEntity> Roles { get; }

        #endregion Roles

        #region Users

        public IReadOnlyList<DefaultPermissionMappingEntity> DefaultPermissionMappings { get; }

        public IReadOnlyList<DefaultRoleMappingEntity> DefaultRoleMappings { get; }

        public IReadOnlyList<UserPermissionMappingEntity> UserPermissionMappings { get; }

        public IReadOnlyList<UserRoleMappingEntity> UserRoleMappings { get; }

        public IReadOnlyList<UserEntity> Users { get; }

        #endregion Users
    }
}
