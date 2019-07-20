using System;

namespace Sokan.Yastah.Data.Administration
{
    public class AdministrationActionBrief
    {
        public DateTimeOffset Performed { get; internal set; }

        public string PerformedBy { get; internal set; }
    }
}
