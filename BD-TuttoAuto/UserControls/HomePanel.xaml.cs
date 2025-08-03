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
    /// Logica di interazione per HomePanel.xaml
    /// </summary>
    public partial class HomePanel : UserControl, IReferencesWindow
    {
        public HomePanel()
        {
            InitializeComponent();
        }

        public MainWindow MWindow { get; set; } = null;

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            MWindow.DisplayControl(nameof(SearchPanel));
        }

        private void BtnPrivateArea_Click(object sender, RoutedEventArgs e)
        {
            MWindow.DisplayControl(nameof(PrivateAreaPanel));
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (MWindow != null)
            {
                LblHome.Content = "Benvenuto " + MWindow.LoggedUser.Name;
            }
        }

        private void BtnFinanziamenti_Click(object sender, RoutedEventArgs e)
        {
            MWindow.DisplayControl(nameof(FinanziamentiPanel));
        }
    }
}
