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
    /// Logica di interazione per ProfilePanel.xaml
    /// </summary>
    public partial class ProfilePanel : UserControl, IReferencesWindow
    {
        public ProfilePanel()
        {
            InitializeComponent();
        }

        public string Username { get; set; }

        public MainWindow MWindow { get; set; } = null;

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LblProfile.Content = "Profilo di " + Username;
            if ((bool)e.NewValue && MWindow != null)
            {
                bool state = Username == MWindow.LoggedUser.Name;
                BtnConversations.IsEnabled = state;
                BtnPrivateArea.IsEnabled = state;
            }
        }

        private void BtnConversations_Click(object sender, RoutedEventArgs e)
        {
            MWindow.DisplayControl(nameof(ChatPanel));
        }

        private void BtnPrivateArea_Click(object sender, RoutedEventArgs e)
        {
            MWindow.DisplayControl(nameof(PrivateAreaPanel));
        }
    }
}
