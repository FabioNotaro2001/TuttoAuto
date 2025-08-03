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
    /// Logica di interazione per SearchPanel.xaml
    /// </summary>
    public partial class SearchPanel : UserControl, IReferencesWindow
    {
        public SearchPanel()
        {
            InitializeComponent();
        }

        public MainWindow MWindow { get; set; } = null;

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            decimal kmax = 0;
            double minprice = 0, maxprice = 0;
            if (!(CmbModel.SelectedIndex != -1 && CmbRegion.SelectedIndex != -1 && CmbManufacturer.SelectedIndex != -1 &&
                    CmbFuel.SelectedIndex != -1 && decimal.TryParse(TxtKMax.Text, out kmax) && double.TryParse(TxtPriceFrom.Text, out minprice) &&
                    double.TryParse(TxtPriceTo.Text, out maxprice))) {
                MessageBox.Show("Impostare tutti i campi con valori validi per fare una ricerca", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var panel = MWindow.UscSearchResult;
            panel.Visualization = SearchResultsPanel.VisualizationType.SEARCHRESULT;
            panel.Model = (string)MWindow.UscSearch.CmbModel.SelectedItem;
            panel.Region = (string)MWindow.UscSearch.CmbRegion.SelectedItem;
            panel.Manufacturer = (string)MWindow.UscSearch.CmbManufacturer.SelectedItem;
            panel.FuelType = (string)MWindow.UscSearch.CmbFuel.SelectedItem;
            panel.MaxKM = kmax;
            panel.MinPrice = minprice;
            panel.MaxPrice = maxprice;
            panel.NeoValid = MWindow.UscSearch.ChkNewDriver.IsChecked == true;
            MWindow.DisplayControl(nameof(SearchResultsPanel));
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && MWindow != null)
            {
                var dbcontext = new TUTTOAUTOEntities();

                CmbManufacturer.SelectedItem = null;
                CmbModel.SelectedItem = null;
                CmbRegion.SelectedItem = null;
                CmbFuel.SelectedItem = null;

                CmbModel.IsEnabled = false;

                var regions = (from r in dbcontext.regioni
                               orderby r.Nome
                               select r.Nome).ToList();
                regions.Add("Tutte");
                CmbRegion.ItemsSource = regions;

                var manufacturers = (from m in dbcontext.case_produttrici
                                     orderby m.Nome
                                     select m.Nome).ToList();
                manufacturers.Add("Tutte");
                CmbManufacturer.ItemsSource = manufacturers;

                var fueltypes = (from a in dbcontext.alimentazioni
                                 orderby a.Nome
                                 select a.Nome).ToList();
                fueltypes.Add("Tutte");
                CmbFuel.ItemsSource = fueltypes;

                TxtKMax.Text = "";
                TxtPriceFrom.Text = "";
                TxtPriceTo.Text = "";
                ChkNewDriver.IsChecked = false;
            }
        }

        private void CmbManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbManufacturer.SelectedItem != null)
            {
                var dbcontext = new TUTTOAUTOEntities();

                CmbModel.SelectedItem = null;
                var models = (from m in dbcontext.modelli
                              where m.CasaProduttrice == (string)CmbManufacturer.SelectedItem
                              orderby m.Nome
                              select m.Nome).ToList();
                models.Add("Tutti");
                CmbModel.ItemsSource = models;
                CmbModel.IsEnabled = true;
            } else
            {
                CmbModel.ItemsSource = null;
                CmbModel.IsEnabled = false;
            }
        }
    }
}
