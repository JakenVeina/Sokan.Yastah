using System;
using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Business.Authorization;

namespace Sokan.Yastah.Business.Test.Authorization
{
    [TestFixture]
    public class InsufficientPermissionsErrorTests
    {
        #region Constructor() Tests

        [Test]
        public void Constructor_MissingPermissionsIsNull_ThrowsException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                _ = new InsufficientPermissionsError(null);
            });
        }

        internal static readonly IReadOnlyList<TestCaseData> Constructor_MissingPermissionsIsNotNull_TestCaseData
            = new[]
            {
                /*                  missingPermissions                                                                                      */
                new TestCaseData(   new Dictionary<int, string>()                                                                           ).SetName("{m}(Empty set)"),
                new TestCaseData(   new Dictionary<int, string>() { { int.MinValue, string.Empty } }                                        ).SetName("{m}(Min Values)"),
                new TestCaseData(   new Dictionary<int, string>() { { 1, "Permission 1" } }                                                 ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   new Dictionary<int, string>() { { 2, "Permission 2" }, { 3, "Permission 3" } }                          ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   new Dictionary<int, string>() { { 4, "Permission 4" }, { 5, "Permission 5" }, { 6, "Permission 6" } }   ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   new Dictionary<int, string>() { { int.MaxValue, "MaxValue" } }                                          ).SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(Constructor_MissingPermissionsIsNotNull_TestCaseData))]
        public void Constructor_Always_SetsMissingPermissions(
            IReadOnlyDictionary<int, string> missingPermissions)
        {
            var result = new InsufficientPermissionsError(
                missingPermissions);

            result.MissingPermissions.ShouldBeSetEqualTo(missingPermissions);
        }

        #endregion Constructor() Tests
    }
}
