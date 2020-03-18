namespace Sokan.Yastah.Common
{
    // Why the hell would this be a useful warning? Seriously. Just because I'm using hex? Go soak your head, this isn't a flags enum.
    #pragma warning disable CA1027 // Mark enums with FlagsAttribute
    public enum ApplicationLogEventType
    #pragma warning restore CA1027 // Mark enums with FlagsAttribute
    {
        Common      = 0x01000000,
        Data        = 0x02000000,
        Business    = 0x03000000,
        Api         = 0x04000000
    }
}
