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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.ComponentModel;
using OpenPop.Mime;
using OpenPop.Mime.Header;
using OpenPop.Pop3.Exceptions;
using OpenPop.Common;
using OpenPop.Common.Logging;
using OpenPop.Mime.Decode;
using OpenPop.Pop3;
using System.IO;
using System.Windows.Threading;

namespace xdirgraf
{
    /// <summary>
    /// Логика взаимодействия для Load.xaml
    /// </summary>
    public partial class Load : MetroWindow
    {
        private string pass, login;
        private BackgroundWorker worker = new BackgroundWorker();
        List<string[]> listEmal = new List<string[]>();
        int MaxCountMeail, NowCountMails;
        public Load()
        {
            InitializeComponent();
        }
        public Load(string Pass, string Login)
        {
            InitializeComponent();
            pass = Pass;
            login = Login;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_Comp;

           var timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0,0,0, 100);
            timer.Start();

            worker.RunWorkerAsync();
        //    backgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
       //     backgroundWorker.RunWorkerAsync();
            
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            CountMessageLabel.Content = NowCountMails.ToString() + " / " + MaxCountMeail.ToString();
        }

        private delegate void myDelegat(int i);
        void progressLoad(int i)
        {
          
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {

          
            string[] words = login.Split(new char[] { '@' });
            string pop3 = "pop." +words[1];
            using (Pop3Client client = new Pop3Client())
            {

                client.Connect(pop3, 995, true);
                client.Authenticate(login, pass);
                int messageCount = client.GetMessageCount();

                MaxCountMeail = messageCount;

                for (int i = messageCount; i > 0; i--)
                {
                    NowCountMails = ( messageCount-i) + 1;
                    System.Threading.Thread.Sleep(10);
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    int z = (messageCount - i) / messageCount * 100;
                    MessageHeader headers = client.GetMessageHeaders(i);
                    RfcMailAddress from = headers.From;
                    string[] buf = new string[3];
                    buf[1] = headers.Subject;
                    buf[0] = from.Address;
                    buf[2] = headers.Date;
                    listEmal.Add(buf);
                    // frr.listBox1.Items.Add("Отправитель: " + from.Address + " Тема: " + HeadersFromAndSubject(pop3, 995, true, login, pass, i));
                //    myDelegat deli = new myDelegat(progressLoad);
                  //  PBarr.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, deli, z);
                }
            }


           
        }
        private void worker_Comp(object sender,RunWorkerCompletedEventArgs e)
        {
           if(!e.Cancelled)
            {
                var frm = new ReadEmail(pass, login,listEmal);
                frm.Show();
                this.Close();
             
            }
        }

        void LoadEmail()
        {

        }
    }
}
