using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data
{
    public class DataAlreadyDeletedError
        : OperationError
    {
        public DataAlreadyDeletedError(string dataDescription)
            : base($"Data already deleted: {dataDescription}") { }
    }
}
