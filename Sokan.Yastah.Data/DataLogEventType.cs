using Sokan.Yastah.Common;

namespace Sokan.Yastah.Data
{
    public enum DataLogEventType
    {
        Repositories    = ApplicationLogEventType.Data + 0x010000,
        Administration  = ApplicationLogEventType.Data + 0x020000,
        Authentication  = ApplicationLogEventType.Data + 0x030000,
        Characters      = ApplicationLogEventType.Data + 0x040000,
        Concurrency     = ApplicationLogEventType.Data + 0x050000,
        Transactions    = ApplicationLogEventType.Data + 0x060000,
        Permissions     = ApplicationLogEventType.Data + 0x070000,
        Roles           = ApplicationLogEventType.Data + 0x080000,
        Users           = ApplicationLogEventType.Data + 0x090000
    }
}
