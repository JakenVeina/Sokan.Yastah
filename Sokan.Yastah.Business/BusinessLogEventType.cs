using Sokan.Yastah.Common;

namespace Sokan.Yastah.Business
{
    internal enum BusinessLogEventType
    {
        Operations      = ApplicationLogEventType.Business + 0x010000,
        Administration  = ApplicationLogEventType.Business + 0x020000,
        Authentication  = ApplicationLogEventType.Business + 0x030000,
        Authorization   = ApplicationLogEventType.Business + 0x040000,
        Characters      = ApplicationLogEventType.Business + 0x050000,
        Permissions     = ApplicationLogEventType.Business + 0x060000,
        Roles           = ApplicationLogEventType.Business + 0x070000,
        Users           = ApplicationLogEventType.Business + 0x080000
    }
}
