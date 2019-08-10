using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Authorization
{
    public class UnauthenticatedUserError
        : OperationErrorBase
    {
        public UnauthenticatedUserError()
            : base("Anonymous users are not allowed to perform this operation") { }
    }
}
