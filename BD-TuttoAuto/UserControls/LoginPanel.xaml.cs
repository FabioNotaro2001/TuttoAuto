using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Linq;
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
    /// Logica di interazione per LoginPanel.xaml
    /// </summary>
    public partial class LoginPanel : UserControl, IReferencesWindow
    {
        public LoginPanel()
        {
            InitializeComponent();
        }
        public MainWindow MWindow { get; set; } = null;
        private bool Start { get; set; } = true;

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!MWindow.IsLoggedIn)
            {
                var dbcontext = new TUTTOAUTOEntities();

                var res = (from u in dbcontext.utenti
                        where u.IndirizzoMail == TxtUser.Text && u.PasswordAccesso == PwdUser.Password
                        select new { u.IndirizzoMail, u.Nominativo }).FirstOrDefault();

                if (res == default)
                {
                    MessageBox.Show("Password invalida o utente inesistente.", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                MessageBox.Show("Accesso eseguito con successo!", "Accesso completato", MessageBoxButton.OK, MessageBoxImage.Information);
                MWindow.IsLoggedIn = true;
                MWindow.LoggedUser = (res.Nominativo, res.IndirizzoMail);
                MWindow.DisplayControl(nameof(HeaderBar));
                MWindow.DisplayControl(nameof(HomePanel));
            }
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!Start && (bool)e.NewValue)
            {
                TxtUser.Text = "";
                PwdUser.Password = "";
            } else if (Start)
            {
                Start = false;
            }
        }
    }
}
