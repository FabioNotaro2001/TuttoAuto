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
    /// Logica di interazione per NewContractPanel.xaml
    /// </summary>
    public partial class NewContractPanel : UserControl, IReferencesWindow
    {
        public NewContractPanel()
        {
            InitializeComponent();
        }

        public (string PlateNum, string Model) Car { get; set; }
        public MainWindow MWindow { get; set; } = null;
        private bool ContractBought { get; set; } = false;

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dbcontext = new TUTTOAUTOEntities();

            if ((bool)e.NewValue && MWindow != null)
            {
                BtnBuy.IsEnabled = true;

                var contracts = from o in dbcontext.offerte_assicurative
                                join asc in dbcontext.assicurazioni
                                on o.PartitaIVAAssicurazione equals asc.PartitaIVA
                                where o.NomeModello == Car.Model
                                select new Contract { Nome = asc.Nome, PartitaIVAAssicurazione = o.PartitaIVAAssicurazione, Massimale = o.Massimale, Premio = o.Premio };
                DtgAvlbContracts.ItemsSource = contracts.ToList();
            }
        }

        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (DtgAvlbContracts.SelectedItem != null && DtgAvlbContracts.SelectedItem is Contract ctr && !ContractBought)
            {
                var dbcontext = new TUTTOAUTOEntities();

                var car = (from cr in dbcontext.auto
                          where cr.Targa == Car.PlateNum
                          select cr).First();

                var stip = (from s in dbcontext.stipulazioni
                            where s.PartitaIVAAssicurazione == ctr.PartitaIVAAssicurazione && s.MailUtente == MWindow.LoggedUser.Mail
                            select s).FirstOrDefault();
                int stipNum = stip != default ? stip.NumeroContrattiTotali : 0;

                var discount = (from d in dbcontext.assicurazioni
                                join s in dbcontext.sconti
                                on d.PartitaIVA equals s.PartitaIVAAssicurazione
                                where s.ContrattiPrecedenti <= stipNum
                                orderby s.PercentualeSconto descending
                                select s.PercentualeSconto).FirstOrDefault();

                dbcontext.contratti_assicurativi.Add(new contratti_assicurativi
                {
                    CasaProduttrice = car.CasaProduttrice,
                    NomeModello = car.NomeModello,
                    DataInizio = DateTime.Today,
                    DataFine = DateTime.Today.AddYears(1),  // Durata di default
                    MailUtente = MWindow.LoggedUser.Mail,
                    PartitaIVAAssicurazione = ctr.PartitaIVAAssicurazione,
                    TargaAuto = Car.PlateNum,
                    PercentualeSconto = discount
                });

                try
                {
                    int complete = dbcontext.SaveChanges();
                    if (complete > 0)
                    {
                        ContractBought = true;
                        BtnBuy.IsEnabled = false;

                        if (stipNum == 0)
                        {
                            dbcontext.stipulazioni.Add(new stipulazioni
                            {
                                MailUtente = MWindow.LoggedUser.Mail,
                                NumeroContrattiTotali = 1,
                                PartitaIVAAssicurazione = ctr.PartitaIVAAssicurazione
                            });
                        }
                        else
                        {
                            stip.NumeroContrattiTotali++;
                        }
                        try
                        {
                            complete = dbcontext.SaveChanges();
                            if (complete > 0)
                            {
                                MessageBox.Show("Contratto comprato con successo!", "Operazione completata", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Errore di gestione della stipulazione.", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Errore di gestione della stipulazione.", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Errore durante l'acquisto del contratto.", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                } 
                catch
                {
                    MessageBox.Show("Errore durante l'acquisto del contratto.", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            // MWindow.DisplayControl(nameof(PrivateAreaPanel));   // TODO: Questo non ci va alla fine.
        }

        sealed internal class Contract
        {
            public string Nome { get; set; }
            public DateTime? DataScadenza { get; set; }
            public double Massimale { get; set; }
            public double Premio { get; set; }
            public string PartitaIVAAssicurazione { get; set; }
        }

        private void DtgAvlbContracts_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "PartitaIVAAssicurazione")
            {
                e.Column = null;
            }
        }
    }
}
