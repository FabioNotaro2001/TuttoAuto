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
    /// Logica di interazione per FinanziamentiPanel.xaml
    /// </summary>
    public partial class FinanziamentiPanel : UserControl, IReferencesWindow
    {
        public MainWindow MWindow { get; set; } = null;

        public FinanziamentiPanel()
        {
            InitializeComponent();
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && MWindow != null)
            {
                var dbcontext = new TUTTOAUTOEntities();

                var fn = from f in dbcontext.proposte_finanziamento
                         select new { f.Banca, f.ImportoSingolaRata, f.NumeroRate, f.Interessi };

                DtgFinanziamenti.ItemsSource = fn.ToList();
            }
        }
    }
}
