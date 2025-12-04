using System;

namespace ProjectReport.Services
{
    public class NavigationService
    {
        private static NavigationService _instance;
        public static NavigationService Instance => _instance ??= new NavigationService();

        private NavigationService() { }

        public event EventHandler<NavigationEventArgs> NavigationRequested;

        public void NavigateToHome()
        {
            NavigationRequested?.Invoke(this, new NavigationEventArgs(NavigationTarget.Home));
        }

        public void NavigateToWellData(int wellId)
        {
            NavigationRequested?.Invoke(this, new NavigationEventArgs(NavigationTarget.WellData, wellId));
        }

        public void NavigateToGeometry(int wellId)
        {
            NavigationRequested?.Invoke(this, new NavigationEventArgs(NavigationTarget.Geometry, wellId));
        }
    }
}
