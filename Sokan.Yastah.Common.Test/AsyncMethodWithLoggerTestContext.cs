namespace Sokan.Yastah.Common.Test
{
    public class AsyncMethodWithLoggerTestContext
        : AsyncMethodTestContext
    {
        protected readonly TestLoggerFactory LoggerFactory
            = new TestLoggerFactory();

        protected override void Dispose(
            bool disposeManaged)
        {
            if(disposeManaged)
                LoggerFactory.Dispose();
            base.Dispose(disposeManaged);
        }
    }
}
