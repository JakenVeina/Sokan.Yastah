using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Business.Roles;

namespace Sokan.Yastah.Business.Test.Roles
{
    [TestFixture]
    public class NameInUseErrorTests
    {
        #region Constructor() Tests

        [TestCase("")]
        [TestCase("name")]
        public void Constructor_Always_MessageContainsName(
            string name)
        {
            var result = new NameInUseError(name);

            result.Message.ShouldContain(name);
        }

        #endregion Constructor() Tests
    }
}
