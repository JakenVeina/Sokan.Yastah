using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data
{
    public class DataAlreadyDeletedError
        : OperationErrorBase
    {
        public DataAlreadyDeletedError(string dataDescription)
            : base($"Data already deleted: {dataDescription}") { }
    }
}
