using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data
{
    public class DataNotFoundError
        : OperationErrorBase
    {
        public DataNotFoundError(string dataDescription)
            : base($"Data not found: {dataDescription}") { }
    }
}
