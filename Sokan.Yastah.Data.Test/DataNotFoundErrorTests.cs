using NUnit.Framework;
using Shouldly;

namespace Sokan.Yastah.Data.Test
{
    [TestFixture]
    public class DataNotFoundErrorTests
    {
        #region Constructor() Tests

        [TestCase("")]
        [TestCase("dataDescription")]
        public void Constructor_Always_InitializesMessage(string dataDescription)
        {
            var result = new DataNotFoundError(dataDescription);

            result.Message.ShouldContain(dataDescription);
        }

        #endregion Constructor() Tests
    }
}
