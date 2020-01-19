using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data
{
    public class NoChangesGivenError
        : OperationError
    {
        public NoChangesGivenError(string dataDescription)
            : base($"No changes given: {dataDescription}") { }
    }
}
