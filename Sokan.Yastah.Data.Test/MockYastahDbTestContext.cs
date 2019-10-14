using System;
using System.Collections.Generic;
using System.Text;

using Moq;

using Sokan.Yastah.Data.Concurrency;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Data.Test
{
    internal class MockYastahDbTestContext
        : AsyncMethodTestContext
    {
        public MockYastahDbTestContext(bool isReadOnly = true)
        {
            Entities = isReadOnly
                ? YastahTestEntitySet.Default
                : new YastahTestEntitySet();

            MockConcurrencyResolutionService = new Mock<IConcurrencyResolutionService>();

            MockContext = new MockYastahDbContext(
                Entities,
                MockConcurrencyResolutionService.Object);
        }

        public readonly YastahTestEntitySet Entities;

        public readonly Mock<IConcurrencyResolutionService> MockConcurrencyResolutionService;

        public readonly MockYastahDbContext MockContext;
    }
}
