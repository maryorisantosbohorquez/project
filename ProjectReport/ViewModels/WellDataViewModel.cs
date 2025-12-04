using ProjectReport.Models;

namespace ProjectReport.ViewModels
{
    public class WellDataViewModel
    {
        private readonly Project _project;
        public Well CurrentWell { get; private set; }

        public WellDataViewModel(Project project)
        {
            _project = project;
        }

        public void LoadWell(Well well)
        {
            CurrentWell = well;
            // ... carga adicional según necesites ...
        }

        // ...añade métodos/properties reales según tu diseño...
    }
}
