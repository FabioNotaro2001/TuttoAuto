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
    /// Logica di interazione per AutoPanel.xaml
    /// </summary>
    public partial class AutoPanel : UserControl, IReferencesWindow
    {
        public AutoPanel()
        {
            InitializeComponent();
        }

        public MainWindow MWindow { get; set; } = null;
        public (string OwnerMail, string PlateNum, DateTime? AdDate) Car;

        private void BtnNewContract_Click(object sender, RoutedEventArgs e)
        {
            var dbcontext = new TUTTOAUTOEntities();

            var model = (from c in dbcontext.auto
                         where c.Targa == Car.PlateNum
                         select c.NomeModello).First();
            MWindow.UscNewContract.Car = (Car.PlateNum, model);
            MWindow.DisplayControl(nameof(NewContractPanel));
        }

        private void BtnContact_Click(object sender, RoutedEventArgs e)
        {
            var dbcontext = new TUTTOAUTOEntities();

            var name = (from u in dbcontext.utenti
                       where u.IndirizzoMail == Car.OwnerMail
                       select u.Nominativo).First();

            MWindow.UscMessage.Receiver = (Car.OwnerMail, name);
            MWindow.DisplayControl(nameof(MessagePanel));
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dbcontext = new TUTTOAUTOEntities();

            if ((bool)e.NewValue && MWindow != null)
            {
                bool notsame = Car.OwnerMail != MWindow.LoggedUser.Mail;

                bool interact;

                var date = (from a in dbcontext.annunci_vendita
                            where a.TargaAuto == Car.PlateNum
                            orderby a.DataAnnuncio descending
                            select a.DataVendita).FirstOrDefault();

                if (notsame)
                {
                    BtnInteract.Content = "Compra";

                    interact = date == default;     // L'auto si può comprare solo se l'annuncio è aperto.
                } else
                {
                    BtnInteract.Content = "Crea\nannuncio";

                    interact = date != default;    // L'auto si può mettere in vendita solo se non c'è un annuncio aperto.
                }

                BtnNewContract.IsEnabled = !notsame;
                BtnContact.IsEnabled = notsame;
                BtnInteract.IsEnabled = interact;

                var car = (from a in dbcontext.auto
                           where a.Targa == Car.PlateNum
                           select new { a.NomeModello, a.Kilometraggio, a.CasaProduttrice, a.Potenza, a.Alimentazione, a.Colore, a.AnnoImmatricolazione }).FirstOrDefault();

                string region = "";
                if (Car.AdDate != null)
                {
                    region = (from a in dbcontext.annunci_vendita
                              where a.DataAnnuncio == Car.AdDate && a.MailVenditore == Car.OwnerMail && a.TargaAuto == Car.PlateNum
                              select a.Regione).FirstOrDefault();
                }

                if (car != default)
                {
                    TxtColor.Text = car.Colore;
                    TxtFuel.Text = car.Alimentazione;
                    TxtKm.Text = car.Kilometraggio.ToString();
                    TxtManufacturer.Text = car.CasaProduttrice;
                    TxtModel.Text = car.NomeModello;
                    TxtPlate.Text = Car.PlateNum;
                    TxtPower.Text = car.Potenza.ToString();
                    TxtRegion.Text = region;
                    TxtYear.Text = car.AnnoImmatricolazione.ToString();
                }

                if (!notsame)
                {
                    var res = (from st in dbcontext.contratti_assicurativi
                               where st.TargaAuto == Car.PlateNum
                               orderby st.DataFine descending
                               select new { st.DataFine }).FirstOrDefault();

                    if (res != default && res.DataFine >= DateTime.Now) // Se la macchina ha un contratto attivo allora il bottone 
                    {                                                   // non è utilizzabile.
                        BtnNewContract.IsEnabled = false;
                    }
                }
            }
        }

        private void BtnInteract_Click(object sender, RoutedEventArgs e)
        {
            var dbcontext = new TUTTOAUTOEntities();

            if (Car.OwnerMail == MWindow.LoggedUser.Mail)
            {
                var isStillOwner = (from c in dbcontext.auto
                                    where c.Targa == Car.PlateNum
                                    select c.MailProprietario).First() == MWindow.LoggedUser.Mail;

                if (!isStillOwner)
                {
                    MessageBox.Show("L'automobile selezionata non è di tua proprietà.", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                    MWindow.DisplayControl(nameof(HomePanel));
                }

                MWindow.UscAd.Plate = Car.PlateNum;
                MWindow.DisplayControl(nameof(CreateAdPanel));
            }
            else
            {
                var ad = (from a in dbcontext.annunci_vendita
                          where a.MailVenditore == Car.OwnerMail &&
                          a.TargaAuto == Car.PlateNum && a.DataAnnuncio == Car.AdDate
                          select a).FirstOrDefault();

                var car = (from c in dbcontext.auto
                           where c.Targa == Car.PlateNum
                           select c).FirstOrDefault();

                if (ad == default || car == default || ad.DataVendita != null)
                {
                    MessageBox.Show("Si è verificato un errore durante l'acquisto.", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                } 
                else
                {
                    ad.DataVendita = DateTime.Now;
                    car.MailProprietario = MWindow.LoggedUser.Mail;

                    dbcontext.SaveChanges();
                    MessageBox.Show("Acquisto completato.", "SUCCESSO", MessageBoxButton.OK, MessageBoxImage.Information);
                    MWindow.DisplayControl(nameof(HomePanel));
                }
            }
        }
    }
}
