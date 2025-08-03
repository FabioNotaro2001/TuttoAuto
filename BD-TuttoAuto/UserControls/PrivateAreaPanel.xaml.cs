using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BD_TuttoAuto.UserControls
{
    /// <summary>
    /// Logica di interazione per PrivateAreaPanel.xaml
    /// </summary>
    public partial class PrivateAreaPanel : UserControl, IReferencesWindow
    {
        public PrivateAreaPanel()
        {
            InitializeComponent();
        }

        public MainWindow MWindow { get; set; } = null;

        private void BtnCars_Click(object sender, RoutedEventArgs e)
        {
            MWindow.UscSearchResult.Visualization = SearchResultsPanel.VisualizationType.OWNCARS;
            MWindow.DisplayControl(nameof(SearchResultsPanel));
        }

        private void BtnFinesContracts_Click(object sender, RoutedEventArgs e)
        {
            MWindow.DisplayControl(nameof(ContractsFinesPanel));
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            MWindow.UscProfile.Username = MWindow.LoggedUser.Name;
            MWindow.DisplayControl(nameof(ProfilePanel));
        }

        private void BtnOwnAds_Click(object sender, RoutedEventArgs e)
        {
            MWindow.UscSearchResult.Visualization = SearchResultsPanel.VisualizationType.OWNADS;
            MWindow.DisplayControl(nameof(SearchResultsPanel));
        }
    }
}
