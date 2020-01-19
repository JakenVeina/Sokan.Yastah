using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Characters
{
    public class InvalidLevelDefinitionsError
        : OperationError
    {
        public InvalidLevelDefinitionsError(
                string message)
            : base(
                message) { }
    }
}
