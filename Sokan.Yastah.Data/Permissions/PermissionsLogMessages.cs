using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Permissions
{
    public static class PermissionsLogMessages
    {
        public static void CategoryDescriptionsEnumerating(
                ILogger logger)
            => _categoryDescriptionsEnumerating.Invoke(
                logger);
        private static readonly Action<ILogger> _categoryDescriptionsEnumerating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4001, nameof(CategoryDescriptionsEnumerating)),
                    $"Enumerating {nameof(PermissionCategoryDescriptionViewModel)}")
                .WithoutException();

        public static void PermissionIdentitiesEnumerating(
                ILogger logger)
            => _permissionIdentitiesEnumerating.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionIdentitiesEnumerating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4002, nameof(PermissionIdentitiesEnumerating)),
                    $"Enumerating {nameof(PermissionCategoryDescriptionViewModel)}")
                .WithoutException();

        public static void PermissionIdsEnumerating(
                ILogger logger,
                Optional<IReadOnlyCollection<int>> permissionIds)
            => _permissionIdsEnumerating.Invoke(
                logger,
                permissionIds);
        private static readonly Action<ILogger, Optional<IReadOnlyCollection<int>>> _permissionIdsEnumerating
            = LoggerMessage.Define<Optional<IReadOnlyCollection<int>>>(
                    LogLevel.Debug,
                    new EventId(4003, nameof(PermissionIdsEnumerating)),
                    $"Enumerating {nameof(PermissionEntity)} IDs\r\n\tPermissionIds: {{PermissionIds}}")
                .WithoutException();
    }
}
