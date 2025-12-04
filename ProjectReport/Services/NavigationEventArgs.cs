using System;

namespace ProjectReport.Services
{
    public class NavigationEventArgs : EventArgs
    {
        public NavigationTarget Target { get; }
        public int? WellId { get; }

        public NavigationEventArgs(NavigationTarget target, int? wellId = null)
        {
            Target = target;
            WellId = wellId;
        }
    }
}
