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
    /// Logica di interazione per CreateAdPanel.xaml
    /// </summary>
    public partial class CreateAdPanel : UserControl, IReferencesWindow
    {
        public MainWindow MWindow { get; set; } = null;
        public string Plate { get; set; }
        private auto Car { get; set; }
        private bool RegionsSet { get; set; } = false;

        public CreateAdPanel()
        {
            InitializeComponent();
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && MWindow != null)
            {
                var dbcontext = new TUTTOAUTOEntities();

                Car = (from c in dbcontext.auto
                       where c.Targa == Plate
                       select c).FirstOrDefault();

                if (Car == default)
                {
                    MessageBox.Show("Si è verificato un errore.", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                    MWindow.DisplayControl(nameof(HomePanel));
                } else
                {
                    TxtPrezzo.Text = "";
                    
                    if (!RegionsSet)
                    {
                        var regions = (from r in dbcontext.regioni
                                       orderby r.Nome
                                       select r.Nome).ToList();
                        CmbRegion.ItemsSource = regions;

                        RegionsSet = true;
                    }
                }
            }
        }

        private void BtnPubblica_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(TxtPrezzo.Text, out double price) && price > 0 && CmbRegion.SelectedItem != null)
            {
                var dbcontext = new TUTTOAUTOEntities();

                dbcontext.annunci_vendita.Add(new annunci_vendita {
                    DataAnnuncio = DateTime.Today, 
                    MailVenditore = MWindow.LoggedUser.Mail,
                    Prezzo = price,
                    Regione = CmbRegion.SelectedItem.ToString(),
                    TargaAuto = Car.Targa
                });

                dbcontext.SaveChanges();

                MessageBox.Show("Annuncio pubblicato.", "SUCCESSO", MessageBoxButton.OK, MessageBoxImage.Information);
                MWindow.DisplayControl(nameof(HomePanel));
            }
            else
            {
                MessageBox.Show("Selezionare una regione e impostare un prezzo valido.", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
