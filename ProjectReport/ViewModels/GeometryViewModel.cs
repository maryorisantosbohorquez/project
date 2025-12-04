using ProjectReport.Models;

namespace ProjectReport.ViewModels
{
    public class GeometryViewModel
    {
        private Well _currentWell;

        public void LoadWell(Well well)
        {
            _currentWell = well;
            // ... inicializa datos de geometría ...
        }

        public void SaveToWell()
        {
            if (_currentWell == null) return;
            // ... guarda datos de geometría en _currentWell ...
        }
    }
}
