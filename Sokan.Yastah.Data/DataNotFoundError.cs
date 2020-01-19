using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data
{
    public class DataNotFoundError
        : OperationError
    {
        public DataNotFoundError(string dataDescription)
            : base($"Data not found: {dataDescription}") { }
    }
}
