using Sokan.Yastah.Common;

namespace Sokan.Yastah.Api
{
    internal enum ApiLogEventType
    {
        Antiforgery     = ApplicationLogEventType.Api + 0x010000,
        Authentication  = ApplicationLogEventType.Api + 0x020000,
        Controller      = ApplicationLogEventType.Api + 0x030000
    }
}
