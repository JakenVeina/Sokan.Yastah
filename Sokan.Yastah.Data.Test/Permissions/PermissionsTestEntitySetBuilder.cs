using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Test.Permissions
{
    internal class PermissionsTestEntitySetBuilder
        : YastahTestEntitySetBuilder
    {
        public static readonly YastahTestEntitySet SharedSet
            = new PermissionsTestEntitySetBuilder().Build();

        private PermissionsTestEntitySetBuilder() { }

        protected override IReadOnlyList<PermissionEntity> CreatePermissions()
            => Enumerable.Empty<PermissionEntity>()
                .Append(new PermissionEntity(   permissionId: 1,    categoryId: 1,  name: "Permission 1, Category 1",   description: "Permission #1, within Permissions Category #1"    ))
                .Append(new PermissionEntity(   permissionId: 2,    categoryId: 2,  name: "Permission 1, Category 2",   description: "Permission #1, within Permissions Category #2"    ))
                .Append(new PermissionEntity(   permissionId: 3,    categoryId: 2,  name: "Permission 2, Category 2",   description: "Permission #2, within Permissions Category #2"    ))
                .Append(new PermissionEntity(   permissionId: 4,    categoryId: 1,  name: "Permission 2, Category 1",   description: "Permission #2, within Permissions Category #1"    ))
                .Append(new PermissionEntity(   permissionId: 5,    categoryId: 3,  name: "Permission 1, Category 3",   description: "Permission #1, within Permissions Category #3"    ))
                .Append(new PermissionEntity(   permissionId: 6,    categoryId: 2,  name: "Permission 3, Category 2",   description: "Permission #3, within Permissions Category #2"    ))
                .Append(new PermissionEntity(   permissionId: 7,    categoryId: 1,  name: "Permission 3, Category 1",   description: "Permission #3, within Permissions Category #1"    ))
                .Append(new PermissionEntity(   permissionId: 8,    categoryId: 2,  name: "Permission 4, Category 2",   description: "Permission #4, within Permissions Category #2"    ))
                .Append(new PermissionEntity(   permissionId: 9,    categoryId: 3,  name: "Permission 2, Category 3",   description: "Permission #2, within Permissions Category #3"    ))
                .ToArray();

        protected override IReadOnlyList<PermissionCategoryEntity> CreatePermissionCategories()
            => Enumerable.Empty<PermissionCategoryEntity>()
                .Append(new PermissionCategoryEntity(   id: 1,  name:   "Category 1",   description: "Permissions Category #1"   ))
                .Append(new PermissionCategoryEntity(   id: 2,  name:   "Category 2",   description: "Permissions Category #2"   ))
                .Append(new PermissionCategoryEntity(   id: 3,  name:   "Category 3",   description: "Permissions Category #3"   ))
                .ToArray();
    }
}
