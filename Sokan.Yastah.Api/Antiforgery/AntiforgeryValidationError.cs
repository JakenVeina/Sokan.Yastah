using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Api.Antiforgery
{
    public class AntiforgeryValidationError
        : OperationErrorBase
    {
        public static readonly AntiforgeryValidationError Default
            = new AntiforgeryValidationError();

        public AntiforgeryValidationError()
            : base("The antiforgery token was either not present, or invalid") { }
    }
}
