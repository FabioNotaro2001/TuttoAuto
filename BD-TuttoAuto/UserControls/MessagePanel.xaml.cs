using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Timer = System.Timers.Timer;

namespace BD_TuttoAuto.UserControls
{
    /// <summary>
    /// Logica di interazione per MessagePanel.xaml
    /// </summary>
    public partial class MessagePanel : UserControl, IReferencesWindow
    {
        public MessagePanel()
        {
            InitializeComponent();

            Messages = new ObservableCollection<Message>();
            _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            FetchTimer.Elapsed += FetchMessages;
            DtgChat.ItemsSource = Messages;
        }

        private void FetchMessages(object sender, ElapsedEventArgs e)
        {
            LoadMessages();
        }

        public MainWindow MWindow { get; set; } = null;
        public (string Mail, string Name) Receiver { get; set; }
        private Timer FetchTimer { get; set; } = new Timer(10000);
        private ObservableCollection<Message> Messages { get; set; }
        private int NewestCode { get; set; } = -1;
        private System.Windows.Threading.Dispatcher _dispatcher;

        private void LoadMessages()
        {
            var dbcontext = new TUTTOAUTOEntities();

            var msgs = from m in dbcontext.messaggi
                       join u in dbcontext.utenti
                       on m.MailMittente equals u.IndirizzoMail
                       where ((m.MailMittente == MWindow.LoggedUser.Mail && m.MailRicevente == Receiver.Mail) ||
                                (m.MailMittente == Receiver.Mail && m.MailRicevente == MWindow.LoggedUser.Mail)) && m.Codice > NewestCode
                       select new { m.Codice, Message = new Message { Nominativo = u.Nominativo, Data = m.Data, Testo = m.Testo } };

            var listmsgs = msgs.ToList();
            int num = listmsgs.Count;
            if (num > 0)
            {
                NewestCode = (from m in listmsgs
                              select m.Codice).Max();

                var list = (from m in listmsgs
                            select m.Message).ToList();

                if (_dispatcher.Thread != Thread.CurrentThread)
                {
                    _dispatcher.Invoke(() =>
                    {
                        foreach (var msg in list)
                        {
                            Messages.Add(msg);
                        }
                    });
                }
                else
                {
                    foreach (var msg in list)
                    {
                        Messages.Add(msg);
                    }
                }
            }
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && MWindow != null)
            {
                LblChat.Content = "Conversazione con " + Receiver.Name;

                LoadMessages();

                FetchTimer.Start();
            } else
            {
                FetchTimer.Stop();
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (TxtMessage.Text.Trim().Length > 0)
            {
                var dbcontext = new TUTTOAUTOEntities();

                string text = TxtMessage.Text;
                dbcontext.messaggi.Add(new messaggi()
                {
                    MailMittente = MWindow.LoggedUser.Mail,
                    MailRicevente = Receiver.Mail,
                    Testo = text,
                    Data = DateTime.Now
                });
                dbcontext.SaveChanges();
                LoadMessages();
                FetchTimer.Stop();
                FetchTimer.Start();

                TxtMessage.Text = "";
            }
        }

        sealed internal class Message
        {
            public string Nominativo { get; set; }
            public DateTime Data { get; set; }
            public string Testo { get; set; }

        }
    }
}
