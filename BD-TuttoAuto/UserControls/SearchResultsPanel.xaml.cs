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
    /// Logica di interazione per SearchResultsPanel.xaml
    /// </summary>
    public partial class SearchResultsPanel : UserControl, IReferencesWindow
    {
        public SearchResultsPanel()
        {
            InitializeComponent();
        }
        
        public MainWindow MWindow { get; set; } = null;
        public VisualizationType Visualization { get; set; }
        public string Model { get; set; }
        public string Region { get; set; }
        public string Manufacturer { get; set; }
        public string FuelType { get; set; }
        public decimal MaxKM { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public bool NeoValid { get; set; }

        private void BtnSelectCar_Click(object sender, RoutedEventArgs e)
        {
            if (DtgSearchResults.SelectedItem != null && DtgSearchResults.SelectedItem is Car c)
            {
                if (c is AdCar ac)
                {
                    MWindow.UscAutoView.Car = (ac.MailProprietario, ac.Targa, ac.DataAnnuncio);
                }
                else
                {
                    MWindow.UscAutoView.Car = (MWindow.LoggedUser.Mail, c.Targa, null);
                }
                MWindow.DisplayControl(nameof(AutoPanel));
            }
        }

        public enum VisualizationType
        {
            SEARCHRESULT, OWNCARS, OWNADS
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && MWindow != null)
            {
                var dbcontext = new TUTTOAUTOEntities();

                switch (Visualization)
                {
                    case VisualizationType.SEARCHRESULT:
                        LblSearch.Content = "Risultati Ricerca";
                        
                        var cars = from av in dbcontext.annunci_vendita
                                   join c in dbcontext.auto
                                   on av.TargaAuto equals c.Targa
                                   join u in dbcontext.utenti
                                   on av.MailVenditore equals u.IndirizzoMail
                                   where av.DataVendita == null &&
                                            av.Prezzo >= MinPrice && av.Prezzo <= MaxPrice &&
                                            (Model == "Tutti" || c.NomeModello == Model) && (Region == "Tutte" || av.Regione == Region) &&
                                            (Manufacturer == "Tutte" || c.CasaProduttrice == Manufacturer) && (FuelType == "Tutte" || c.Alimentazione == FuelType) &&
                                            c.Kilometraggio <= MaxKM && (!NeoValid || c.Potenza <= 70)
                                            select new AdCar
                                            {
                                                MailProprietario = av.MailVenditore,
                                                NomeProprietario = u.Nominativo,
                                                Prezzo = av.Prezzo,
                                                DataAnnuncio = av.DataAnnuncio,
                                                Kilometraggio = c.Kilometraggio,
                                                Alimentazione = c.Alimentazione,
                                                Potenza = c.Potenza,
                                                AnnoImmatricolazione = c.AnnoImmatricolazione,
                                                NomeModello = c.NomeModello,
                                                CasaProduttrice = c.CasaProduttrice,
                                                Colore = c.Colore,
                                                Targa = c.Targa
                                            };

                        DtgSearchResults.ItemsSource = cars.ToList();
                        break;
                    case VisualizationType.OWNCARS:
                        LblSearch.Content = "Le tue auto";

                        var ownCars = from c in dbcontext.auto
                                      where c.MailProprietario == MWindow.LoggedUser.Mail
                                      select new Car
                                      {
                                          NomeModello = c.NomeModello,
                                          Kilometraggio = c.Kilometraggio,
                                          Potenza = c.Potenza,
                                          Alimentazione = c.Alimentazione,
                                          AnnoImmatricolazione = c.AnnoImmatricolazione,
                                          Colore = c.Colore,
                                          CasaProduttrice = c.CasaProduttrice,
                                          Targa = c.Targa
                                      };
                        DtgSearchResults.ItemsSource = ownCars.ToList();
                        break;
                    case VisualizationType.OWNADS:
                        LblSearch.Content = "I tuoi annunci";

                        var adscars = from av in dbcontext.annunci_vendita
                                   join c in dbcontext.auto
                                   on av.TargaAuto equals c.Targa
                                   join u in dbcontext.utenti
                                   on av.MailVenditore equals u.IndirizzoMail
                                   where u.IndirizzoMail == MWindow.LoggedUser.Mail
                                   select new OwnAdCar
                                   {
                                       MailProprietario = MWindow.LoggedUser.Mail,
                                       Concluso = av.DataVendita != null,
                                       DataVendita = av.DataVendita,
                                       Prezzo = av.Prezzo,
                                       DataAnnuncio = av.DataAnnuncio,
                                       NomeProprietario = u.Nominativo,
                                       Kilometraggio = c.Kilometraggio,
                                       Potenza = c.Potenza,
                                       Alimentazione = c.Alimentazione,
                                       AnnoImmatricolazione = c.AnnoImmatricolazione,
                                       NomeModello = c.NomeModello,
                                       CasaProduttrice = c.CasaProduttrice,
                                       Colore = c.Colore,
                                       Targa = c.Targa
                                   };

                        DtgSearchResults.ItemsSource = adscars.ToList();
                        break;
                }

            }
        }

        internal class Car
        {
            public string NomeModello { get; set; }
            public int Potenza { get; set; }
            public string Alimentazione { get; set; }
            public decimal Kilometraggio { get; set; }
            public int AnnoImmatricolazione { get; set; }
            public string CasaProduttrice { get; set; }
            public string Colore { get; set; }
            public string Targa { get; set; }
        }

        internal class AdCar : Car
        {
            public double Prezzo { get; set; }
            public DateTime DataAnnuncio { get; set; }
            public string NomeProprietario { get; set; }
            public string MailProprietario { get; set; }
        }

        internal class OwnAdCar : AdCar
        {
            public bool Concluso { get; set; }
            public DateTime? DataVendita { get; set; }
        }

        private void DtgSearchResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Targa" || e.PropertyName == "MailProprietario") {    // Nasconde delle colonne nel DataGrid.
                e.Column = null;
            }
        }
    }
}
