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
            MockAdministrationActionCategorySet = SetupMockSet(
                entities.AdministrationActionCategories,
                (keys, entity) => entity.Id == (int)keys[0]);
                
            MockAdministrationActionTypeSet = SetupMockSet(
                entities.AdministrationActionTypes,
                (keys, entity) => entity.Id == (int)keys[0]);

            MockAdministrationActionSet = SetupMockSet(
                entities.AdministrationActions,
                (keys, entity) => entity.Id == (long)keys[0]); 

            MockAuthenticationTicketSet = SetupMockSet(
                entities.AuthenticationTickets,
                (keys, entity) => entity.Id == (long)keys[0]); 

            MockPermissionCategorySet = SetupMockSet(
                entities.PermissionCategories,
                (keys, entity) => entity.Id == (int)keys[0]); 

            MockPermissionSet = SetupMockSet(
                entities.Permissions,
                (keys, entity) => entity.PermissionId == (int)keys[0]); 

            MockRolePermissionMappingSet = SetupMockSet(
                entities.RolePermissionMappings,
                (keys, entity) => entity.Id == (long)keys[0]); 

            MockRoleVersionSet = SetupMockSet(
                entities.RoleVersions,
                (keys, entity) => entity.Id == (long)keys[0]); 

            MockRoleSet = SetupMockSet(
                entities.Roles,
                (keys, entity) => entity.Id == (long)keys[0]); 

            MockDefaultPermissionMappingSet = SetupMockSet(
                entities.DefaultPermissionMappings,
                (keys, entity) => entity.Id == (long)keys[0]); 

            MockDefaultRoleMappingSet = SetupMockSet(
                entities.DefaultRoleMappings,
                (keys, entity) => entity.Id == (long)keys[0]); 

            MockUserPermissionMappingSet = SetupMockSet(
                entities.UserPermissionMappings,
                (keys, entity) => entity.Id == (long)keys[0]); 

            MockUserRoleMappingSet = SetupMockSet(
                entities.UserRoleMappings,
                (keys, entity) => entity.Id == (long)keys[0]); 

            MockUserSet = SetupMockSet(
                entities.Users,
                (keys, entity) => entity.Id == (ulong)keys[0]); 
        }

        #region Administration

        public readonly Mock<DbSet<AdministrationActionCategoryEntity>> MockAdministrationActionCategorySet;

        public readonly Mock<DbSet<AdministrationActionTypeEntity>> MockAdministrationActionTypeSet;

        public readonly Mock<DbSet<AdministrationActionEntity>> MockAdministrationActionSet;

        #endregion Administration

        #region Authentication

        public readonly Mock<DbSet<AuthenticationTicketEntity>> MockAuthenticationTicketSet;

        #endregion Authentication

        #region Permission

        public readonly Mock<DbSet<PermissionCategoryEntity>> MockPermissionCategorySet;

        public readonly Mock<DbSet<PermissionEntity>> MockPermissionSet;

        #endregion Permission

        #region Roles

        public readonly Mock<DbSet<RolePermissionMappingEntity>> MockRolePermissionMappingSet;

        public readonly Mock<DbSet<RoleVersionEntity>> MockRoleVersionSet;

        public readonly Mock<DbSet<RoleEntity>> MockRoleSet;

        #endregion Roles

        #region Users

        public readonly Mock<DbSet<DefaultPermissionMappingEntity>> MockDefaultPermissionMappingSet;

        public readonly Mock<DbSet<DefaultRoleMappingEntity>> MockDefaultRoleMappingSet;

        public readonly Mock<DbSet<UserPermissionMappingEntity>> MockUserPermissionMappingSet;

        public readonly Mock<DbSet<UserRoleMappingEntity>> MockUserRoleMappingSet;

        public readonly Mock<DbSet<UserEntity>> MockUserSet;

        #endregion Users

        private Mock<DbSet<TEntity>> SetupMockSet<TEntity>(
                IReadOnlyList<TEntity> entities,
                Func<object[], TEntity, bool> findPredicate)
            where TEntity : class
        {
            var mockSet = entities
                .AsQueryable()
                .BuildMockDbSet();
            
            Setup(x => x.Set<TEntity>())
                .Returns(() => mockSet.Object);
            
            Setup(x => x.FindAsync<TEntity>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] keys, CancellationToken cancellationToken)
                    => Task.FromResult(entities.FirstOrDefault(entity => findPredicate.Invoke(keys, entity))));

            return mockSet;
        }
    }
}
