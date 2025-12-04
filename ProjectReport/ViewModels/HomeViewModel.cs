using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using ProjectReport.Models;

namespace ProjectReport.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly Project _project;
        private readonly ICollectionView _wellsView;

        public HomeViewModel(Project project)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));

            // Vista filtrable/ordenable sobre la colección de pozos
            _wellsView = CollectionViewSource.GetDefaultView(_project.Wells);
            _wellsView.Filter = FilterWells;

            // Opciones de orden
            SortOptions = new ObservableCollection<string>
            {
                "Well Name (A-Z)",
                "Well Name (Z-A)",
                "Last Modified (Newest)",
                "Last Modified (Oldest)",
                "Spud Date (Newest)",
                "Spud Date (Oldest)"
            };
            _selectedSortOption = "Last Modified (Newest)";
            ApplySorting();

            // Comandos
            NewWellCommand      = new RelayCommand(_ => CreateNewWell());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            ToggleViewCommand   = new RelayCommand(_ => ToggleView());

            // Stats iniciales
            UpdateDashboardStatistics();

            // Recalcular stats cuando cambian los pozos
            _project.Wells.CollectionChanged += (_, __) => UpdateDashboardStatistics();
        }

        // ========= PROPIEDADES DE VISTA =========

        public ICollectionView WellsView => _wellsView;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _wellsView.Refresh();
                }
            }
        }

        private bool _isCardView;
        public bool IsCardView
        {
            get => _isCardView;
            set => SetProperty(ref _isCardView, value);
        }

        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (SetProperty(ref _selectedSortOption, value))
                {
                    ApplySorting();
                }
            }
        }

        public ObservableCollection<string> SortOptions { get; }

        // ========= KPIs =========

        private int _totalWells;
        public int TotalWells
        {
            get => _totalWells;
            set => SetProperty(ref _totalWells, value);
        }

        private int _draftWells;
        public int DraftWells
        {
            get => _draftWells;
            set => SetProperty(ref _draftWells, value);
        }

        private int _inProgressWells;
        public int InProgressWells
        {
            get => _inProgressWells;
            set => SetProperty(ref _inProgressWells, value);
        }

        private int _completedWells;
        public int CompletedWells
        {
            get => _completedWells;
            set => SetProperty(ref _completedWells, value);
        }

        private int _archivedWells;
        public int ArchivedWells
        {
            get => _archivedWells;
            set => SetProperty(ref _archivedWells, value);
        }

        private int _activeOperators;
        public int ActiveOperators
        {
            get => _activeOperators;
            set => SetProperty(ref _activeOperators, value);
        }

        private void UpdateDashboardStatistics()
        {
            TotalWells      = _project.Wells.Count;
            DraftWells      = _project.Wells.Count(w => w.Status == WellStatus.Draft);
            InProgressWells = _project.Wells.Count(w => w.Status == WellStatus.InProgress);
            CompletedWells  = _project.Wells.Count(w => w.Status == WellStatus.Completed);
            ArchivedWells   = _project.Wells.Count(w => w.Status == WellStatus.Archived);
            ActiveOperators = _project.Wells.Select(w => w.Operator).Where(op => !string.IsNullOrWhiteSpace(op)).Distinct().Count();
        }

        // ========= COMANDOS =========

        public ICommand NewWellCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand ToggleViewCommand { get; }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedSortOption = "Last Modified (Newest)";
            _wellsView.Refresh();
        }

        private void ToggleView()
        {
            IsCardView = !IsCardView;
        }

        private void CreateNewWell()
        {
            // Generar nuevo Id incremental
            int newId = _project.Wells.Count > 0
                ? _project.Wells.Max(w => w.Id) + 1
                : 1;

            var now = DateTime.Now;

            var newWell = new Well
            {
                Id           = newId,
                WellName     = $"New Well {newId}",
                Operator     = string.Empty,
                Field        = string.Empty,
                Block        = string.Empty,
                Status       = WellStatus.Draft,
                CreatedDate  = now,
                LastModified = now,
                SpudDate     = now
            };

            _project.AddWell(newWell);
            _project.SetActiveWell(newWell.Id);

            _wellsView.Refresh();
            UpdateDashboardStatistics();

            // Aquí, en el proyecto original, llamarías a:
            // - DataPersistenceService.SaveProject(...)
            // - NavigationService.Instance.NavigateToWellData(newWell.Id);
            // - ToastNotificationService.Instance.ShowSuccess(...)
            // Eso lo enchufamos después cuando tengas esos servicios listos.
        }

        // ========= FILTRO =========

        private bool FilterWells(object obj)
        {
            if (obj is not Well well)
                return false;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.ToLower();
                if (!well.WellName.ToLower().Contains(s) &&
                    !well.Operator.ToLower().Contains(s) &&
                    !well.Field.ToLower().Contains(s) &&
                    !well.Block.ToLower().Contains(s))
                {
                    return false;
                }
            }

            return true;
        }

        // ========= ORDEN =========

        private void ApplySorting()
        {
            _wellsView.SortDescriptions.Clear();

            switch (SelectedSortOption)
            {
                case "Well Name (A-Z)":
                    _wellsView.SortDescriptions.Add(new SortDescription(nameof(Well.WellName), ListSortDirection.Ascending));
                    break;
                case "Well Name (Z-A)":
                    _wellsView.SortDescriptions.Add(new SortDescription(nameof(Well.WellName), ListSortDirection.Descending));
                    break;
                case "Last Modified (Newest)":
                    _wellsView.SortDescriptions.Add(new SortDescription(nameof(Well.LastModified), ListSortDirection.Descending));
                    break;
                case "Last Modified (Oldest)":
                    _wellsView.SortDescriptions.Add(new SortDescription(nameof(Well.LastModified), ListSortDirection.Ascending));
                    break;
                case "Spud Date (Newest)":
                    _wellsView.SortDescriptions.Add(new SortDescription(nameof(Well.SpudDate), ListSortDirection.Descending));
                    break;
                case "Spud Date (Oldest)":
                    _wellsView.SortDescriptions.Add(new SortDescription(nameof(Well.SpudDate), ListSortDirection.Ascending));
                    break;
            }

            _wellsView.Refresh();
        }
    }
}
