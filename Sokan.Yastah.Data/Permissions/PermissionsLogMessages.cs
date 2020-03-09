using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Permissions
{
    public static class PermissionsLogMessages
    {
        public enum EventType
        {
            CategoryDescriptionsEnumerating = DataLogEventType.Permissions + 0x0001,
            PermissionIdentitiesEnumerating = DataLogEventType.Permissions + 0x0002,
            PermissionIdsEnumerating        = DataLogEventType.Permissions + 0x0003
        }

        public static void CategoryDescriptionsEnumerating(
                ILogger logger)
            => _categoryDescriptionsEnumerating.Invoke(
                logger);
        private static readonly Action<ILogger> _categoryDescriptionsEnumerating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CategoryDescriptionsEnumerating.ToEventId(),
                    $"Enumerating {nameof(PermissionCategoryDescriptionViewModel)}")
                .WithoutException();

        public static void PermissionIdentitiesEnumerating(
                ILogger logger)
            => _permissionIdentitiesEnumerating.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionIdentitiesEnumerating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.PermissionIdentitiesEnumerating.ToEventId(),
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
                    EventType.PermissionIdsEnumerating.ToEventId(),
                    $"Enumerating {nameof(PermissionEntity)} IDs\r\n\tPermissionIds: {{PermissionIds}}")
                .WithoutException();
    }
}
