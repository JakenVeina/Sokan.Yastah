using NUnit.Framework;
using Shouldly;

namespace Sokan.Yastah.Data.Test
{
    [TestFixture]
    public class DataAlreadyDeletedErrorTests
    {
        #region Constructor() Tests

        [TestCase("")]
        [TestCase("dataDescription")]
        public void Constructor_Always_InitializesMessage(string dataDescription)
        {
            var result = new DataAlreadyDeletedError(dataDescription);

            result.Message.ShouldContain(dataDescription);
        }

        #endregion Constructor() Tests
    }
}
