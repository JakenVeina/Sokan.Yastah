using NUnit.Framework;

using Sokan.Yastah.Business.Authorization;

namespace Sokan.Yastah.Business.Test.Authorization
{
    [TestFixture]
    public class UnauthenticatedUserErrorTests
    {
        #region Constructor() Tests

        [Test]
        public void Constructor_Always_RunsSuccessfully()
        {
            var _ = new UnauthenticatedUserError();
        }

        #endregion Constructor() Tests
    }
}
