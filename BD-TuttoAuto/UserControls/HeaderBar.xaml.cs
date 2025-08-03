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
    /// Logica di interazione per HeaderBar.xaml
    /// </summary>
    public partial class HeaderBar : UserControl, IReferencesWindow
    {
        public HeaderBar()
        {
            InitializeComponent();
        }

        public MainWindow MWindow { get; set; } = null;

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            MWindow.DisplayControl(nameof(HomePanel));
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            MWindow.IsLoggedIn = false;
            MWindow.DisplayControl(nameof(LoginPanel));
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            MWindow.DisplayControl(nameof(SearchPanel));
        }

        private void BtnPrivateArea_Click(object sender, RoutedEventArgs e)
        {
            MWindow.DisplayControl(nameof(PrivateAreaPanel));
        }
    }
}
