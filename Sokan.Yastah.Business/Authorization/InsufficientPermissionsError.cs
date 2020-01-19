using System;
using System.Collections.Generic;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Authorization
{
    public class InsufficientPermissionsError
        : OperationError
    {
        public InsufficientPermissionsError(
                IReadOnlyDictionary<int, string> missingPermissions)
            : base("Insufficient permissions to perform this operation")
        {
            MissingPermissions = missingPermissions;
        }

        public IReadOnlyDictionary<int, string> MissingPermissions { get; }
    }
}
