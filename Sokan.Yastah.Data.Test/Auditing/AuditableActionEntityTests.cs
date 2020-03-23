using System;
using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Auditing;

namespace Sokan.Yastah.Data.Test.Auditing
{
    [TestFixture]
    public class AuditableActionEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             typeId          performed,                          performedById   */
                new TestCaseData(   default(long),  default(int),   default(DateTimeOffset),            default(ulong?) ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  int.MinValue,   DateTimeOffset.MinValue,            ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2,              DateTimeOffset.Parse("0003-04-05"), 6UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   7L,             8,              DateTimeOffset.Parse("0009-10-11"), 12UL            ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   13L,            14,             DateTimeOffset.Parse("0015-04-17"), 18UL            ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  int.MaxValue,   DateTimeOffset.MaxValue,            ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsEntity(
            long id,
            int typeId,
            DateTimeOffset performed,
            ulong? performedById)
        {
            var result = new AuditableActionEntity(
                id,
                typeId,
                performed,
                performedById);

            result.Id.ShouldBe(id);
            result.TypeId.ShouldBe(typeId);
            result.Performed.ShouldBe(performed);
            result.PerformedById.ShouldBe(performedById);
        }

        #endregion Constructor() Tests
    }
}
