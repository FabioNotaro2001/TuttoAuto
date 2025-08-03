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
    /// Logica di interazione per ContractsFinePanel.xaml
    /// </summary>
    public partial class ContractsFinesPanel : UserControl, IReferencesWindow
    {
        public ContractsFinesPanel()
        {
            InitializeComponent();
        }

        public MainWindow MWindow { get; set; } = null;

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && MWindow != null)
            {
                var dbcontext = new TUTTOAUTOEntities();

                var fines = from f in dbcontext.multe
                            where f.MailUtente == MWindow.LoggedUser.Mail
                            select new { f.DataInfrazione, f.Costo, f.DataScadenza, f.CittàInfrazione, f.ViaInfrazione, f.Descrizione, f.NumeroVerbale };
                DtgFines.ItemsSource = fines.ToList();

                var contracts = from c in dbcontext.contratti_assicurativi
                                where c.MailUtente == MWindow.LoggedUser.Mail
                                select new { Attivo = c.DataFine > DateTime.Now, c.DataInizio, c.DataFine, ModelloAuto = c.NomeModello, c.TargaAuto };
                DtgContracts.ItemsSource = contracts.ToList();
            }
        }
    }
}
