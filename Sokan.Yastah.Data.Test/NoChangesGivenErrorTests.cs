using NUnit.Framework;
using Shouldly;

namespace Sokan.Yastah.Data.Test
{
    [TestFixture]
    public class NoChangesGivenErrorTests
    {
        #region Constructor() Tests

        [TestCase("")]
        [TestCase("dataDescription")]
        public void Constructor_Always_InitializesMessage(string dataDescription)
        {
            var result = new NoChangesGivenError(dataDescription);

            result.Message.ShouldContain(dataDescription);
        }

        #endregion Constructor() Tests
    }
}
