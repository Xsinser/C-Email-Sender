﻿using System;
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
using System.IO;
using System.Windows.Threading;
using XsenoCode;
using MailKit.Security;
using MailKit.Net.Smtp;
using MailKit.Net.Imap;
using MailKit.Search;
using System.Net;
using System.Threading;
using MailKit;
using MimeKit;
using System.Data.SQLite;
namespace xdirgraf
{
    /// <summary>
    /// Логика взаимодействия для SentWindow.xaml
    /// </summary>
    public partial class SentWindow : MetroWindow
    {
        public SentWindow(string Pass, string Login, List<string[]> ListEmail)
        {

            InitializeComponent();

          try
          { 
            pass = Pass;
            login = Login;
            listEmail = ListEmail;
            listEmailAdresses();
            id = loginCheck();
            GetGroups();
            GetNames();
            Group.ItemsSource = GroupBd;
            AllSender.ItemsSource = NamesAdr;
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Вернитесь в меню!");
                AllSender.IsEnabled = Filter.IsEnabled = Group.IsEnabled = false;
            }
        }

        private string login;

        private string pass;

        private string selectedEmailAdress;

        private List<string[]> listEmail;

        private string BufferMailText;

        private List<string> listWithDataToProtect = new List<string>();

        private List<string> Mails = new List<string>();
        private List<string> GroupBd = new List<string>();
        private List<string> NamesAdr = new List<string>();
        private List<string> NamesGroup = new List<string>();


        int listType = 0;
        bool CloseCheck = false;
        int id;
        int loginCheck()
        {
            var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
            con.Open();
            var com = new SQLiteCommand("Select * from Members", con);
            var read = com.ExecuteReader();

            while (read.Read())
            {
                if (read[1].ToString() == login)
                {

                    return Convert.ToInt16(read[0].ToString());
                }
            }
            con.Close();
            return -1;


        }
        void GetGroups()
        {
            var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
            con.Open();
            var com = new SQLiteCommand("Select * from type_address", con);
            var read = com.ExecuteReader();

            while (read.Read())
            {
                GroupBd.Add(read[0].ToString());
            }
            con.Close();
        }
        void GetNames()
        {
            var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
            con.Open();
            var com = new SQLiteCommand("Select * from Address_Data where  member='" + id + "'", con);
            var read = com.ExecuteReader();

            while (read.Read())
            {
                NamesAdr.Add(read[1].ToString());
                NamesGroup.Add(read[4].ToString());
            }
            con.Close();
        }
        private void listEmailAdresses()
        {
            List<string> bul = new List<string>();
            foreach (var buferMassString in listEmail)
            {
                if (ListCheck(buferMassString[0], bul) && EmailCheck(buferMassString[0]))
                {
                    bul.Add(buferMassString[0]);

                }
            }
            AllSender.ItemsSource = null;
            AllSender.ItemsSource = bul;
        }
        bool EmailCheck(string EmailString)
        {
            foreach (string bufs in Mails)
            {
                if (bufs == EmailString)
                    return true;
            }
            return false;
        }
        private bool ListCheck(string turgetString, List<string> bul)
        {

            foreach (var bufs in bul)
            {
                if (turgetString == bufs)
                    return false;
            }
            return true;

        }





        void addListHeaderEmail()
        {
            listWithDataToProtect.Clear();

            int bufi = AllSender.SelectedIndex;
            List<string> bufList = new List<string>();
            selectedEmailAdress = AllSender.Items.GetItemAt(bufi).ToString();
            foreach (var buferMassString in listEmail)
            {

                if (AllSender.Items.GetItemAt(bufi).ToString() == buferMassString[0])
                {
                    bufList.Add(buferMassString[1]);
                    listWithDataToProtect.Add(buferMassString[2]);
                }

            }
            AllSender.ItemsSource = bufList;
            AllSender.Visibility = Visibility.Visible;
        }

        void getMails(string TargetStr)
        {
            Mails.Clear();
            string[] words = TargetStr.Split(new char[] { ' ' });
            for (int i = 0; i < words.Length; i++)
            {
                Mails.Add(words[i]);
            }

        }

        void addListEmailThisAddress()
        {
            try
            {
                listWithDataToProtect.Clear();

                if (AllSender.SelectedIndex != -1)
                {

                    AllSender.Visibility = Visibility.Hidden;
                    string bufs = AllSender.SelectedItem.ToString();

                    var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
                    con.Open();
                    var com = new SQLiteCommand("Select * from Address_Data where Name='" + bufs + "' and  member='" + id + "'", con);
                    var read = com.ExecuteReader();
                    Mails.Clear();
                    read.Read();
                    {

                        getMails(read[2].ToString());
                        listEmailAdresses();

                    }
                    con.Close();
                }
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Вернитесь в меню!");
                AllSender.IsEnabled = Filter.IsEnabled = Group.IsEnabled = false;
            }

        }


        string checkDataList()
        {
            string bus = "";

            int i = 0;

            foreach (var bufsl in listWithDataToProtect)
            {
                if (i == AllSender.SelectedIndex)
                {
                    bus = bufsl;
                    break;
                }
                else
                    i++;
            }

            return bus;
        }
        int checkList()
        {
            int i = 0;
            int bufi = AllSender.SelectedIndex;
            string bufhe = AllSender.Items.GetItemAt(bufi).ToString();
            string bufData = checkDataList();
            foreach (var bufs in listEmail)
            {
                if ((bufs[0] == selectedEmailAdress) && (bufs[1] == bufhe) && (bufData == bufs[2]))
                {
                    Email.Text = bufs[1];
                    Header.Text = bufs[0];
                    break;
                }
                else
                {
                    i++;
                }
            }
            return i;
        }


        private void AllSender_MouseEnter(object sender, MouseEventArgs e)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////
            if (AllSender.SelectedIndex >= 0)
            {
                switch (listType)
                {
                    case 0:

                        //   addListHeaderEmail();
                        addListEmailThisAddress();
                        listType++;
                        AllSender.Visibility = Visibility.Visible;
                        BackCon.Visibility = Visibility.Visible;
                        Filter.IsEnabled = false;
                        Group.IsEnabled = false;
                        break;
                    case 1:
                        addListHeaderEmail();
                        listType++;
                        AllSender.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        /// addListHeaderEmail();
                        getEmail();
                        AllSender.Visibility = Visibility.Visible;
                        break;
                }
                ;
            }

        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            var mm = new MainMenu(pass, login);
            CloseCheck = true;
            mm.Show();
            this.Close();
        }


        private void BackCon_Click(object sender, RoutedEventArgs e)
        {
            listType--;
            if (listType == 1)
                listEmailAdresses();           
            if (listType == 0)
            {
                NamesAdr.Clear();
                Filter.Clear();
                GetNames();
                Group.Text = "";
                Filter.IsEnabled = true;
                Group.IsEnabled = true;
                Group.ItemsSource = GroupBd;
                AllSender.ItemsSource = NamesAdr;
                BackCon.Visibility = Visibility.Hidden;
            }

        }

        private void Decripter_Checked(object sender, EventArgs e)
        {
            string bufs = new TextRange(TextMessag.Document.ContentStart, TextMessag.Document.ContentEnd).Text;
            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            char ch = '!';

            if (bufs.IndexOf(ch) == -1)
            {
                if (Convert.ToBoolean(Decripter.IsChecked))
                {
                    XenoCode a = new XenoCode();

                    paragraph.Inlines.Add(new Bold(new Run(a.decryption(bufs, login, selectedEmailAdress))));
                }
                else

                    paragraph.Inlines.Add(new Bold(new Run(BufferMailText)));


                document.Blocks.Add(paragraph);
                TextMessag.Document = document;
            }
            else
            {
                string bufsMail = PasswordMailCheck(bufs);
                if (bufsMail != "")
                {

                    if (Convert.ToBoolean(Decripter.IsChecked))
                    {
                        XenoCode a = new XenoCode();

                        paragraph.Inlines.Add(new Bold(new Run(a.decryption(bufsMail, login, selectedEmailAdress))));
                    }
                    else
                    {
                        paragraph.Inlines.Add(new Bold(new Run(BufferMailText)));

                    }
                    document.Blocks.Add(paragraph);
                    TextMessag.Document = document;
                }
            }
        }

        string PasswordMailCheck(string bufs)
        {
            XenoCode a = new XenoCode();



            string[] hh = bufs.Split(new char[] { '!' });
            string b = hh[0];
            string d = a.decryption(b, login, selectedEmailAdress);
            PasswordWindow passwordWindow = new PasswordWindow();
            string bufReplaceS = hh[1];
            if (passwordWindow.ShowDialog() == true)
            {
                if (d == passwordWindow.Password)
                    return bufReplaceS;

                else
                {
                    Decripter.IsChecked = false;
                    return "";
                }
            }
            else
            {
                Decripter.IsChecked = false;
                return "";
            }

        }

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Group.Text == "")
                AllSender.ItemsSource = FilterBox(Filter.Text);
            else
                if(Group.SelectedIndex>=0)
                    AllSender.ItemsSource = FilterBoxWithGroup(Filter.Text);
                
        }
        List<string> FilterBox(string sometext)
        {
            List<string> bufList = new List<string>();
            foreach (string bufs in NamesAdr)
            {
                if (bufs.IndexOf(sometext) > -1)
                    bufList.Add(bufs);
            }
            return bufList;
        }
        List<string> FilterBoxWithGroup(string sometext)
        {
            List<string> bufList = new List<string>();
            int i = 0;
            foreach (string bufs in NamesAdr)
            {
                if ((bufs.IndexOf(sometext) > -1) && (NamesGroup[i] == Group.SelectedItem.ToString()))
                    bufList.Add(bufs);
                i++;
            }
            return bufList;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CloseCheck)
                System.Windows.Application.Current.Shutdown();
        }

        private void Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Group.Text == "")
                AllSender.ItemsSource = FilterGroup(Filter.Text);
            else
                if (Group.SelectedIndex >= 0)
                AllSender.ItemsSource = FilterBoxWithGroup(Filter.Text);
        }
        List<string> FilterGroup(string sometext)
        {
            List<string> bufList = new List<string>();
            int i = 0;
            if(Group.SelectedIndex>-1)
            foreach (string bufs in NamesAdr)
            {
                if ((bufs.IndexOf(sometext) > -1) && (NamesGroup[i] == Group.SelectedItem.ToString()))
                    bufList.Add(bufs);
                i++;
            }
            return bufList;
        }
        /// ////////////////////////////////////////////////////////////////  /// ////////////////////////////////////////////////////////////////  /// ////////////////////////////////////////////////////////////////  /// ////////////////////////////////////////////////////////////////
        /// ////////////////////////////////////////////////////////////////  /// ////////////////////////////////////////////////////////////////
        void getEmail()
        {

            try
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
                            {


                                var message = inbox.GetMessage(inbox.Count - checkList()-1, cancel.Token);
                                if (message.HtmlBody == null)
                                {
                                    Paragraph paragraph = new Paragraph();
                                    FlowDocument document = new FlowDocument();
                                    paragraph.Inlines.Add(new Bold(new Run(message.TextBody)));
                                    BufferMailText = message.TextBody;
                                    document.Blocks.Add(paragraph);
                                    TextMessag.Document = document;

                                }
                                else
                                {

                                    Paragraph paragraph = new Paragraph();
                                    FlowDocument document = new FlowDocument();
                                    string buferstring = message.HtmlBody.Replace("<br>", "\n");
                                    buferstring = buferstring.Replace("<HTML>", ""); buferstring = buferstring.Replace("</HTML>", "");
                                    buferstring = buferstring.Replace("<BODY>", ""); buferstring = buferstring.Replace("</BODY>", "");
                                    buferstring = buferstring.Replace("<p>", ""); buferstring = buferstring.Replace("</p>", "");
                                    buferstring = buferstring.Replace("<div>", ""); buferstring = buferstring.Replace("</div>", "");
                                    paragraph.Inlines.Add(new Bold(new Run(buferstring)));
                                    BufferMailText = message.TextBody; 
                                    document.Blocks.Add(paragraph);
                                    TextMessag.Document = document;

                                }

                            }
                            client.Disconnect(true, cancel.Token);
                        }

                    }
                }
            }
           catch
            {
               MessageBox.Show("Произошла ошибка! Вернитесь в меню!");
                AllSender.IsEnabled = Filter.IsEnabled = Group.IsEnabled = false;
            }
        }



    }
}