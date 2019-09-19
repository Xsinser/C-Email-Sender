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
using MailKit.Security;
using MailKit.Net.Smtp;
using MailKit.Net.Imap;
using MailKit.Search;
using System.Net;
using System.Threading;
using MailKit;
using MimeKit;
namespace xdirgraf
{
    /// <summary>
    /// Логика взаимодействия для LoadImap.xaml
    /// </summary>
    public partial class LoadImap : MetroWindow
    {
       

        private string pass, login;
        private BackgroundWorker worker = new BackgroundWorker();
        List<string[]> listEmal = new List<string[]>();
        int MaxCountMeail, NowCountMails;
    public LoadImap()
        {
            InitializeComponent();
        }
public LoadImap(string Pass, string Login)
        {
            InitializeComponent();
            pass = Pass;
            login = Login;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_Comp;

            var timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
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
            using (var client = new ImapClient())
            {
                using (var cancel = new CancellationTokenSource())
                {
                    string[] words = login.Split(new char[] { '@' });
                    string imap = "imap." + words[1];
                    client.Connect(imap, 993, true, cancel.Token);

                    client.AuthenticationMechanisms.Remove("XOAUTH");

                    client.Authenticate(login, pass, cancel.Token);


                    if ((client.Capabilities & (ImapCapabilities.SpecialUse | ImapCapabilities.XList)) != 0)
                    {
                        var inbox = client.GetFolder(SpecialFolder.Sent);

                        inbox.Open(FolderAccess.ReadOnly, cancel.Token);

                        int Count = inbox.Count;
                        MaxCountMeail = Count;

                        for (int i = Count - 1; i >= 0; i--)
                        {
                            //        string[] buf = new string[3];
                            //        buf[1] = headers.Subject;
                            //        buf[0] = from.Address;
                            //        buf[2] = headers.Date;
                            //        listEmal.Add(buf);
                            NowCountMails = (Count - i);

                            System.Threading.Thread.Sleep(10);
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            var message = inbox.GetMessage(i, cancel.Token);
                            string[] buf = new string[3];
                            int starti = message.To.ToString().IndexOf(" <");
                            string bufs = message.To.ToString();
                            bufs = bufs.Remove(0, starti + 1);
                            bufs = bufs.Replace("<", "");
                            bufs = bufs.Replace(">", "");
                            try
                            {
                                buf[1] = message.Subject.ToString();
                            }
                            catch
                            {
                                buf[1] = "без темы";
                            }
                            buf[0] = bufs;
                            buf[2] = message.Date.ToString();
                            listEmal.Add(buf);



                        }
                        client.Disconnect(true, cancel.Token);
                    }

                    // frr.listBox1.Items.Add("Отправитель: " + from.Address + " Тема: " + HeadersFromAndSubject(pop3, 995, true, login, pass, i));
                    //    myDelegat deli = new myDelegat(progressLoad);
                    //  PBarr.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, deli, z);
                }
            }

            //string[] words = login.Split(new char[] { '@' });
            //string pop3 = "pop." + words[1];
            //using (Pop3Client client = new Pop3Client())
            //{

            //    client.Connect(pop3, 995, true);
            //    client.Authenticate(login, pass);
            //    int messageCount = client.GetMessageCount();

            //    MaxCountMeail = messageCount;

            //    for (int i = messageCount; i > 0; i--)
            //    {
            //        NowCountMails = (messageCount - i) + 1;
            //        System.Threading.Thread.Sleep(10);
            //        if (worker.CancellationPending)
            //        {
            //            e.Cancel = true;
            //            return;
            //        }
            //        int z = (messageCount - i) / messageCount * 100;
            //        MessageHeader headers = client.GetMessageHeaders(i);
            //        RfcMailAddress from = headers.From;
            //        string[] buf = new string[3];
            //        buf[1] = headers.Subject;
            //        buf[0] = from.Address;
            //        buf[2] = headers.Date;
            //        listEmal.Add(buf);




        }
        private void worker_Comp(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                var frm = new SentWindow(pass, login, listEmal);
                frm.Show();
                this.Close();

            }
        }

        void LoadEmail()
        {

        }
    }
}
