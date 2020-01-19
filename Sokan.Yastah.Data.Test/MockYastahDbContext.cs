using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MockQueryable.Moq;
using Moq;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Administration;
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
                () => entities.AdministrationActionCategories,
                (keys, entity) => entity.Id == (int)keys[0]);

            _lazyMockAdministrationActionTypeSet = SetupLazyMockSet(
                () => entities.AdministrationActionTypes,
                (keys, entity) => entity.Id == (int)keys[0]);

            _lazyMockAdministrationActionSet = SetupLazyMockSet(
                () => entities.AdministrationActions,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockAuthenticationTicketSet = SetupLazyMockSet(
                () => entities.AuthenticationTickets,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockCharacterGuildSet = SetupLazyMockSet(
                () => entities.CharacterGuilds,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockCharacterGuildDivisionSet = SetupLazyMockSet(
                () => entities.CharacterGuildDivisions,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockCharacterGuildDivisionVersionSet = SetupLazyMockSet(
                () => entities.CharacterGuildDivisionVersions,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockCharacterGuildVersionSet = SetupLazyMockSet(
                () => entities.CharacterGuildVersions,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockCharacterLevelDefinitionSet = SetupLazyMockSet(
                () => entities.CharacterLevelDefinitions,
                (keys, entity) => entity.Level == (int)keys[0]);

            _lazyMockCharacterLevelDefinitionVersionSet = SetupLazyMockSet(
                () => entities.CharacterLevelDefinitionVersions,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockCharacterSet = SetupLazyMockSet(
                () => entities.Characters,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockCharacterVersionSet = SetupLazyMockSet(
                () => entities.CharacterVersions,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockPermissionCategorySet = SetupLazyMockSet(
                () => entities.PermissionCategories,
                (keys, entity) => entity.Id == (int)keys[0]);

            _lazyMockPermissionSet = SetupLazyMockSet(
                () => entities.Permissions,
                (keys, entity) => entity.PermissionId == (int)keys[0]);

            _lazyMockRolePermissionMappingSet = SetupLazyMockSet(
                () => entities.RolePermissionMappings,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockRoleVersionSet = SetupLazyMockSet(
                () => entities.RoleVersions,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockRoleSet = SetupLazyMockSet(
                () => entities.Roles,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockDefaultPermissionMappingSet = SetupLazyMockSet(
                () => entities.DefaultPermissionMappings,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockDefaultRoleMappingSet = SetupLazyMockSet(
                () => entities.DefaultRoleMappings,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockUserPermissionMappingSet = SetupLazyMockSet(
                () => entities.UserPermissionMappings,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockUserRoleMappingSet = SetupLazyMockSet(
                () => entities.UserRoleMappings,
                (keys, entity) => entity.Id == (long)keys[0]);

            _lazyMockUserSet = SetupLazyMockSet(
                () => entities.Users,
                (keys, entity) => entity.Id == (ulong)keys[0]); 
        }

        #region Administration

        public Mock<DbSet<AdministrationActionCategoryEntity>> MockAdministrationActionCategorySet
            => _lazyMockAdministrationActionCategorySet.Value;
        private readonly Lazy<Mock<DbSet<AdministrationActionCategoryEntity>>> _lazyMockAdministrationActionCategorySet;

        public Mock<DbSet<AdministrationActionTypeEntity>> MockAdministrationActionTypeSet
            => _lazyMockAdministrationActionTypeSet.Value;
        private readonly Lazy<Mock<DbSet<AdministrationActionTypeEntity>>> _lazyMockAdministrationActionTypeSet;

        public Mock<DbSet<AdministrationActionEntity>> MockAdministrationActionSet
            => _lazyMockAdministrationActionSet.Value;
        private readonly Lazy<Mock<DbSet<AdministrationActionEntity>>> _lazyMockAdministrationActionSet;

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
                Func<IReadOnlyList<TEntity>> entitiesGetter,
                Func<object[], TEntity, bool> findPredicate)
            where TEntity : class
        {
            var lazyMockSet = LazyEx.CreateThreadSafe(() =>
            {
                var mockSet = entitiesGetter.Invoke()
                    .AsQueryable()
                    .BuildMockDbSet();

                mockSet.CallBase = true;

                return mockSet;
            });

            Setup(x => x.Set<TEntity>())
                .Returns(() => lazyMockSet.Value.Object);

            Setup(x => x.FindAsync<TEntity>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] keys, CancellationToken cancellationToken)
                    => new ValueTask<TEntity>(entitiesGetter.Invoke().FirstOrDefault(entity => findPredicate.Invoke(keys, entity))));

            return lazyMockSet;
        }
    }
}
