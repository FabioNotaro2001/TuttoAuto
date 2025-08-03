using BD_TuttoAuto.UserControls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace BD_TuttoAuto
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ControlDisplayed = (nameof(LoginPanel), UscLogin);
        }

        public bool IsLoggedIn { get; set; } = false;
        public (string Name, string Mail) LoggedUser { get; set; }
        private Dictionary<string, UserControl> UserControls { get; } = new Dictionary<string, UserControl>();
        private (string ControlName, UserControl Control) ControlDisplayed { get; set; }

        private void RegisterElement(IReferencesWindow control)
        {
            control.MWindow = this;
        }

        public void AddKnownUserControl(UserControl u)
        {
            UserControls.Add(u.GetType().Name, u);
            if (u is IReferencesWindow refWindow)
            {
                RegisterElement(refWindow);
            }
        }

        public void DisplayControl(string controlName)
        {
            if(UserControls.ContainsKey(controlName))
            {
                if (controlName == nameof(LoginPanel))
                {
                    HideCurrentControl();
                    UscHeaderBar.Visibility = Visibility.Collapsed;
                    UscLogin.Visibility = Visibility.Visible;
                }
                else if (controlName == nameof(HeaderBar))
                {
                    UscHeaderBar.Visibility = Visibility.Visible;
                }
                else
                {
                    if (controlName != ControlDisplayed.ControlName)
                    {
                        HideCurrentControl();
                        ControlDisplayed = (controlName, UserControls[controlName]);
                        ShowCurrentControl();
                    }
                }
            }

        }

        public void HideCurrentControl()
        {
            if (ControlDisplayed != default)
            {
                ControlDisplayed.Control.Visibility = Visibility.Collapsed;
            }
        }

        public void ShowCurrentControl()
        {
            if (ControlDisplayed != default)
            {
                ControlDisplayed.Control.Visibility = Visibility.Visible;
            }
        }

        private void UscElement_Loaded(object sender, RoutedEventArgs e)
        {
            AddKnownUserControl(sender as UserControl);
        }
    }
}
