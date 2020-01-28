using Moq;

using Sokan.Yastah.Data.Concurrency;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Data.Test
{
    internal class MockYastahDbTestContext
        : AsyncMethodTestContext
    {
        public MockYastahDbTestContext(
            YastahTestEntitySet entities)
        {
            Entities = entities;

            MockConcurrencyResolutionService = new Mock<IConcurrencyResolutionService>();

            MockContext = new MockYastahDbContext(
                entities,
                MockConcurrencyResolutionService.Object);
        }

        public readonly YastahTestEntitySet Entities;

        public readonly Mock<IConcurrencyResolutionService> MockConcurrencyResolutionService;

        public readonly MockYastahDbContext MockContext;
    }
}
