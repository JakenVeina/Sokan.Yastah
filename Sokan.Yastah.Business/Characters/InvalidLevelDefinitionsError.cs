using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Characters
{
    public class InvalidLevelDefinitionsError
        : OperationErrorBase
    {
        public InvalidLevelDefinitionsError(
                string message)
            : base(
                message) { }
    }
}
