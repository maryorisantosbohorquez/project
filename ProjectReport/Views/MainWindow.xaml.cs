using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using ProjectReport.Models;
using ProjectReport.Services;
using ProjectReport.ViewModels;
using ProjectReport.Views;

namespace ProjectReport.Views
{
    public partial class MainWindow : Window
    {
        private readonly Project _project;
        private readonly HomeViewModel _homeViewModel;

        private GeometryView _geometryView;
        private HomeView _homeView;
        private WellDataView _wellDataView;

        public MainWindow()
        {
            InitializeComponent();

            // Proyecto principal
            _project = new Project
            {
                Name = "Proyecto Demo"
            };

            // ViewModel de Home trabajando sobre este Project
            _homeViewModel = new HomeViewModel(_project);

            // Suscribir navegación global (igual que proyecto original)
            NavigationService.Instance.NavigationRequested += OnNavigationRequested;

            // Cargar Home al iniciar
            NavigateToHome();
        }

        // ----------------------------------------------------------------
        //                       NAVEGACIÓN GLOBAL
        // ----------------------------------------------------------------

        private void OnNavigationRequested(object sender, NavigationEventArgs e)
        {
            switch (e.Target)
            {
                case NavigationTarget.Home:
                    NavigateToHome();
                    break;

                case NavigationTarget.WellData:
                    if (e.WellId.HasValue)
                        NavigateToWellData(e.WellId.Value);
                    break;

                case NavigationTarget.Geometry:
                    if (e.WellId.HasValue)
                        NavigateToGeometry(e.WellId.Value);
                    break;
            }
        }

        private void NavigateToHome()
        {
            SaveGeometryDataIfNeeded();

            if (_homeView == null)
            {
                _homeView = new HomeView
                {
                    DataContext = _homeViewModel
                };
            }

            // Cambiado: usar helper para mostrar la vista en el host
            ShowViewInHost(_homeView);
        }

        private void NavigateToWellData(int wellId)
        {
            SaveGeometryDataIfNeeded();

            var well = _project.Wells.FirstOrDefault(w => w.Id == wellId);
            if (well == null)
                return;

            _wellDataView = new WellDataView();
            var vm = new WellDataViewModel(_project);
            vm.LoadWell(well);
            _wellDataView.DataContext = vm;

            // Cambiado: usar helper para mostrar la vista en el host
            ShowViewInHost(_wellDataView);
        }

        private void NavigateToGeometry(int wellId)
        {
            var well = _project.Wells.FirstOrDefault(w => w.Id == wellId);
            if (well == null) return;

            if (_geometryView == null)
                _geometryView = new GeometryView();

            if (_geometryView.DataContext is GeometryViewModel vm)
                vm.LoadWell(well);

            // Mantener uso del helper
            ShowViewInHost(_geometryView);
        }

        // ----------------------------------------------------------------
        //                 GUARDAR GEO ANTES DE CAMBIAR VISTA
        // ----------------------------------------------------------------

        private void SaveGeometryDataIfNeeded()
        {
            if (_geometryView != null &&
                // Cambiado: usar GeometryViewModel del namespace ProjectReport.ViewModels
                _geometryView.DataContext is GeometryViewModel vm)
            {
                vm.SaveToWell();
            }
        }

        // ----------------------------------------------------------------
        //                      EVENTOS DE BOTONES
        // ----------------------------------------------------------------

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToHome();
        }

        private void GeometryButton_Click(object sender, RoutedEventArgs e)
        {
            // Si no hay pozos todavía, no hacemos nada
            if (_project.Wells.Count == 0)
                return;

            int wellId = _project.Wells.First().Id;
            NavigateToGeometry(wellId);
        }

        private void WellDataButton_Click(object sender, RoutedEventArgs e)
        {
            // Si no hay pozos todavía, no hacemos nada
            if (_project.Wells.Count == 0)
                return;

            int wellId = _project.Wells.First().Id;
            NavigateToWellData(wellId);
        }

        protected override void OnClosed(EventArgs e)
        {
            NavigationService.Instance.NavigationRequested -= OnNavigationRequested;
            base.OnClosed(e);
        }

        // Helper: busca un ContentControl host y muestra la vista allí.
        private ContentControl GetContentHost()
        {
            // Nombres habituales usados en XAML; añade los tuyos si tienes otro nombre.
            var namesToTry = new[] { "ContentArea", "MainContent", "MainContentControl", "ContentHost" };
            foreach (var name in namesToTry)
            {
                var found = FindName(name) as ContentControl;
                if (found != null) return found;
            }

            // Fallback: intenta el primer ContentControl dentro del contenido lógico de la ventana
            if (this.Content is ContentControl cc) return cc;

            return null;
        }

        private void ShowViewInHost(object view)
        {
            var host = GetContentHost();
            if (host != null)
            {
                host.Content = view;
            }
            else
            {
                // Último recurso: reemplaza el contenido de la ventana completa
                this.Content = view;
            }
        }
    }
}
