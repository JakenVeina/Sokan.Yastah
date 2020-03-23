using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using MockQueryable.Moq;
using Moq;

using Sokan.Yastah.Data.Auditing;
using Sokan.Yastah.Data.Authentication;
using Sokan.Yastah.Data.Characters;
using Sokan.Yastah.Data.Concurrency;
using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Roles;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test
{
    internal class MockYastahDbContext
        : Mock<YastahDbContext>
    {
        public MockYastahDbContext(
                YastahTestEntitySet entities,
                IConcurrencyResolutionService concurrencyResolutionService)
            : base(concurrencyResolutionService)
        {
            _lazyMockAdministrationActionCategorySet = SetupLazyMockSet(
                entities.AdministrationActionCategories);

            _lazyMockAdministrationActionTypeSet = SetupLazyMockSet(
                entities.AdministrationActionTypes);

            _lazyMockAdministrationActionSet = SetupLazyMockSet(
                entities.AdministrationActions);

            _lazyMockAuthenticationTicketSet = SetupLazyMockSet(
                entities.AuthenticationTickets);

            _lazyMockCharacterGuildSet = SetupLazyMockSet(
                entities.CharacterGuilds);

            _lazyMockCharacterGuildDivisionSet = SetupLazyMockSet(
                entities.CharacterGuildDivisions);

            _lazyMockCharacterGuildDivisionVersionSet = SetupLazyMockSet(
                entities.CharacterGuildDivisionVersions);

            _lazyMockCharacterGuildVersionSet = SetupLazyMockSet(
                entities.CharacterGuildVersions);

            _lazyMockCharacterLevelDefinitionSet = SetupLazyMockSet(
                entities.CharacterLevelDefinitions);

            _lazyMockCharacterLevelDefinitionVersionSet = SetupLazyMockSet(
                entities.CharacterLevelDefinitionVersions);

            _lazyMockCharacterSet = SetupLazyMockSet(
                entities.Characters);

            _lazyMockCharacterVersionSet = SetupLazyMockSet(
                entities.CharacterVersions);

            _lazyMockPermissionCategorySet = SetupLazyMockSet(
                entities.PermissionCategories);

            _lazyMockPermissionSet = SetupLazyMockSet(
                entities.Permissions);

            _lazyMockRolePermissionMappingSet = SetupLazyMockSet(
                entities.RolePermissionMappings);

            _lazyMockRoleVersionSet = SetupLazyMockSet(
                entities.RoleVersions);

            _lazyMockRoleSet = SetupLazyMockSet(
                entities.Roles);

            _lazyMockDefaultPermissionMappingSet = SetupLazyMockSet(
                entities.DefaultPermissionMappings);

            _lazyMockDefaultRoleMappingSet = SetupLazyMockSet(
                entities.DefaultRoleMappings);

            _lazyMockUserPermissionMappingSet = SetupLazyMockSet(
                entities.UserPermissionMappings);

            _lazyMockUserRoleMappingSet = SetupLazyMockSet(
                entities.UserRoleMappings);

            _lazyMockUserSet = SetupLazyMockSet(
                entities.Users); 
        }

        #region Administration

        public Mock<DbSet<AuditableActionCategoryEntity>> MockAdministrationActionCategorySet
            => _lazyMockAdministrationActionCategorySet.Value;
        private readonly Lazy<Mock<DbSet<AuditableActionCategoryEntity>>> _lazyMockAdministrationActionCategorySet;

        public Mock<DbSet<AuditableActionTypeEntity>> MockAdministrationActionTypeSet
            => _lazyMockAdministrationActionTypeSet.Value;
        private readonly Lazy<Mock<DbSet<AuditableActionTypeEntity>>> _lazyMockAdministrationActionTypeSet;

        public Mock<DbSet<AuditableActionEntity>> MockAdministrationActionSet
            => _lazyMockAdministrationActionSet.Value;
        private readonly Lazy<Mock<DbSet<AuditableActionEntity>>> _lazyMockAdministrationActionSet;

        #endregion Administration

        #region Authentication

        public Mock<DbSet<AuthenticationTicketEntity>> MockAuthenticationTicketSet
            => _lazyMockAuthenticationTicketSet.Value;
        private readonly Lazy<Mock<DbSet<AuthenticationTicketEntity>>> _lazyMockAuthenticationTicketSet;

        #endregion Authentication

        #region Characters

        public Mock<DbSet<CharacterGuildEntity>> MockCharacterGuildSet
            => _lazyMockCharacterGuildSet.Value;
        private readonly Lazy<Mock<DbSet<CharacterGuildEntity>>> _lazyMockCharacterGuildSet;

        public Mock<DbSet<CharacterGuildDivisionEntity>> MockCharacterGuildDivisionSet
            => _lazyMockCharacterGuildDivisionSet.Value;
        private readonly Lazy<Mock<DbSet<CharacterGuildDivisionEntity>>> _lazyMockCharacterGuildDivisionSet;

        public Mock<DbSet<CharacterGuildDivisionVersionEntity>> MockCharacterGuildDivisionVersionSet
            => _lazyMockCharacterGuildDivisionVersionSet.Value;
        private readonly Lazy<Mock<DbSet<CharacterGuildDivisionVersionEntity>>> _lazyMockCharacterGuildDivisionVersionSet;

        public Mock<DbSet<CharacterGuildVersionEntity>> MockCharacterGuildVersionSet
            => _lazyMockCharacterGuildVersionSet.Value;
        private readonly Lazy<Mock<DbSet<CharacterGuildVersionEntity>>> _lazyMockCharacterGuildVersionSet;

        public Mock<DbSet<CharacterLevelDefinitionEntity>> MockCharacterLevelDefinitionSet
            => _lazyMockCharacterLevelDefinitionSet.Value;
        private readonly Lazy<Mock<DbSet<CharacterLevelDefinitionEntity>>> _lazyMockCharacterLevelDefinitionSet;

        public Mock<DbSet<CharacterLevelDefinitionVersionEntity>> MockCharacterLevelDefinitionVersionSet
            => _lazyMockCharacterLevelDefinitionVersionSet.Value;
        private readonly Lazy<Mock<DbSet<CharacterLevelDefinitionVersionEntity>>> _lazyMockCharacterLevelDefinitionVersionSet;

        public Mock<DbSet<CharacterEntity>> MockCharacterSet
            => _lazyMockCharacterSet.Value;
        private readonly Lazy<Mock<DbSet<CharacterEntity>>> _lazyMockCharacterSet;

        public Mock<DbSet<CharacterVersionEntity>> MockCharacterVersionSet
            => _lazyMockCharacterVersionSet.Value;
        private readonly Lazy<Mock<DbSet<CharacterVersionEntity>>> _lazyMockCharacterVersionSet;

        #endregion Characters

        #region Permission

        public Mock<DbSet<PermissionCategoryEntity>> MockPermissionCategorySet
            => _lazyMockPermissionCategorySet.Value;
        private readonly Lazy<Mock<DbSet<PermissionCategoryEntity>>> _lazyMockPermissionCategorySet;

        public Mock<DbSet<PermissionEntity>> MockPermissionSet
            => _lazyMockPermissionSet.Value;
        private readonly Lazy<Mock<DbSet<PermissionEntity>>> _lazyMockPermissionSet;

        #endregion Permission

        #region Roles

        public Mock<DbSet<RolePermissionMappingEntity>> MockRolePermissionMappingSet
            => _lazyMockRolePermissionMappingSet.Value;
        private readonly Lazy<Mock<DbSet<RolePermissionMappingEntity>>> _lazyMockRolePermissionMappingSet;

        public Mock<DbSet<RoleVersionEntity>> MockRoleVersionSet
            => _lazyMockRoleVersionSet.Value;
        private readonly Lazy<Mock<DbSet<RoleVersionEntity>>> _lazyMockRoleVersionSet;

        public Mock<DbSet<RoleEntity>> MockRoleSet
            => _lazyMockRoleSet.Value;
        private readonly Lazy<Mock<DbSet<RoleEntity>>> _lazyMockRoleSet;

        #endregion Roles

        #region Users

        public Mock<DbSet<DefaultPermissionMappingEntity>> MockDefaultPermissionMappingSet
            => _lazyMockDefaultPermissionMappingSet.Value;
        private readonly Lazy<Mock<DbSet<DefaultPermissionMappingEntity>>> _lazyMockDefaultPermissionMappingSet;

        public Mock<DbSet<DefaultRoleMappingEntity>> MockDefaultRoleMappingSet
            => _lazyMockDefaultRoleMappingSet.Value;
        private readonly Lazy<Mock<DbSet<DefaultRoleMappingEntity>>> _lazyMockDefaultRoleMappingSet;

        public Mock<DbSet<UserPermissionMappingEntity>> MockUserPermissionMappingSet
            => _lazyMockUserPermissionMappingSet.Value;
        private readonly Lazy<Mock<DbSet<UserPermissionMappingEntity>>> _lazyMockUserPermissionMappingSet;

        public Mock<DbSet<UserRoleMappingEntity>> MockUserRoleMappingSet
            => _lazyMockUserRoleMappingSet.Value;
        private readonly Lazy<Mock<DbSet<UserRoleMappingEntity>>> _lazyMockUserRoleMappingSet;

        public Mock<DbSet<UserEntity>> MockUserSet
            => _lazyMockUserSet.Value;
        private readonly Lazy<Mock<DbSet<UserEntity>>> _lazyMockUserSet;

        #endregion Users

        private Lazy<Mock<DbSet<TEntity>>> SetupLazyMockSet<TEntity>(
                IReadOnlyList<TEntity> entities)
            where TEntity : class
        {
            var lazyMockSet = LazyEx.Create(() =>
            {
                var mockSet = entities
                    .AsQueryable()
                    .BuildMockDbSet();

                mockSet
                    .Setup(x => x.AsQueryable())
                    .Returns(() => mockSet.Object);

                mockSet
                    .Setup(x => x.AsAsyncEnumerable())
                    .Returns(() => mockSet.Object);

                return mockSet;
            });

            Setup(x => x.Set<TEntity>())
                .Returns(() => lazyMockSet.Value.Object);

            return lazyMockSet;
        }
    }
}
