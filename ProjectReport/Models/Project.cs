using System.Collections.ObjectModel;
using System.Linq;

namespace ProjectReport.Models
{
    public class Project
    {
        public string Name { get; set; } = string.Empty;

        public ObservableCollection<Well> Wells { get; } = new ObservableCollection<Well>();

        public int? ActiveWellId { get; private set; }

        public void AddWell(Well well)
        {
            Wells.Add(well);
        }

        public void RemoveWell(int wellId)
        {
            var well = Wells.FirstOrDefault(w => w.Id == wellId);
            if (well != null)
                Wells.Remove(well);
        }

        public void SetActiveWell(int wellId)
        {
            ActiveWellId = wellId;
        }
    }
}
