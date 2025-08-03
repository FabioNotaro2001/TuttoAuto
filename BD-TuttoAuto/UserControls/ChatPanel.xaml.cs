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
    /// Logica di interazione per ChatPanel.xaml
    /// </summary>
    public partial class ChatPanel : UserControl, IReferencesWindow
    {
        public ChatPanel()
        {
            InitializeComponent();
        }

        public MainWindow MWindow { get; set; } = null;

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && MWindow != null)
            {
                var dbcontext = new TUTTOAUTOEntities();

                var mOrdSend = from m2 in dbcontext.messaggi                    // Ultimo messaggio inviato dall'utente per ogni conversazione.
                               where m2.MailMittente == MWindow.LoggedUser.Mail
                               group m2.Data by m2.MailRicevente into g
                               select new { MailRicevente = g.Key, Data = g.ToList().Max() };

                var mOrdReceive = from m2 in dbcontext.messaggi                 // Ultimo messaggio ricevuto dall'utente per ogni conversazione.
                                  where m2.MailRicevente == MWindow.LoggedUser.Mail
                                  group m2.Data by m2.MailMittente into g
                                  select new { MailMittente = g.Key, Data = g.ToList().Max() };

                var usersmsgs = (from m in dbcontext.messaggi                   // Combinazione messaggi ricevuti e inviati.
                                 join mOrd in mOrdSend
                                 on m.MailRicevente equals mOrd.MailRicevente
                                 where m.MailMittente == MWindow.LoggedUser.Mail
                                 select new { User1 = m.MailMittente, User2 = m.MailRicevente, Sender = m.MailMittente, Date = m.Data, Text = m.Testo })
                                .Union(from m in dbcontext.messaggi
                                       join mOrd in mOrdReceive
                                       on m.MailMittente equals mOrd.MailMittente
                                       where m.MailRicevente == MWindow.LoggedUser.Mail
                                       select new { User1 = m.MailRicevente, User2 = m.MailMittente, Sender = m.MailMittente, Date = m.Data, Text = m.Testo });

                var tabmax = from m in usersmsgs                                // Ottenimento messaggio più recente per ogni conversazione.
                             group m.Date by new { m.User1, m.User2 } into g
                             select new { g.Key.User1, g.Key.User2, Date = g.ToList().Max() };

                var convs = from m1 in usersmsgs                                // Ottenimento di tutti i dati dei messaggi più recenti.
                            join t in tabmax
                            on new { m1.User1, m1.User2, m1.Date } equals t
                            select new ConversationRecord { Destinatario = m1.User2, DataMessaggio = m1.Date, Testo = m1.Text };

                DtgChats.ItemsSource = convs.ToList();
            }
        }

        private void BtnOpenChat_Click(object sender, RoutedEventArgs e)
        {
            if (DtgChats.SelectedItem != null && DtgChats.SelectedItem is ConversationRecord conv)
            {
                var dbcontext = new TUTTOAUTOEntities();

                string recvmail = conv.Destinatario;
                var recvname = (from u in dbcontext.utenti
                               where u.IndirizzoMail == recvmail
                               select u.Nominativo).FirstOrDefault();

                if (recvname == default)
                {
                    MessageBox.Show("Si è verificato un errore all'apertura della conversazione", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                MWindow.UscMessage.Receiver = (recvmail, recvname);
                MWindow.DisplayControl(nameof(MessagePanel));
            }
        }

        sealed internal class ConversationRecord
        {
            public string Destinatario { get; set; }
            public DateTime DataMessaggio { get; set; }
            public string Testo { get; set; }
        }
    }
}
