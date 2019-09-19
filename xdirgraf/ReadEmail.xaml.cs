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
using OpenPop.Mime;
using OpenPop.Mime.Header;
using OpenPop.Pop3.Exceptions;
using OpenPop.Common;
using OpenPop.Common.Logging;
using OpenPop.Mime.Decode;
using OpenPop.Pop3;
using System.IO;
using XsenoCode;
using System.Data.SQLite;
namespace xdirgraf
{
    /// <summary>
    /// Логика взаимодействия для ReadEmail.xaml
    /// </summary>
    public partial class ReadEmail : MetroWindow
    {
        private string login;

        private string pass;

        private string selectedEmailAdress;

        private List<string[]> listEmail;

        private List<string> listWithDataToProtect = new List<string>();

        private string BufferMailText;

        private List<string> Mails = new List<string>();
        private List<string> GroupBd = new List<string>();
        private List<string> NamesAdr = new List<string>();
        private List<string> NamesGroup = new List<string>();
        bool CloseCheck = false;
        int listType = 0;
        int id;
        public ReadEmail()
        {
            InitializeComponent();
        }

        public ReadEmail(string Pass, string Login, List<string[]> ListEmail)
        {
            InitializeComponent();
            try
            {
                pass = Pass;
                login = Login;
                listEmail = ListEmail;
                id = loginCheck();
                GetGroups();
                GetNames();
                Group.ItemsSource = GroupBd;
                AllSender.ItemsSource = NamesAdr;
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Вернитесь в меню!");
                AllSender.IsEnabled = Filter.IsEnabled = Group.IsEnabled=false;
            }
         
        }
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
                if (ListCheck(buferMassString[0], bul)&& EmailCheck(buferMassString[0]))
                {
                    bul.Add(buferMassString[0]);

                }
            }
            AllSender.ItemsSource = null;
            AllSender.ItemsSource = bul;
        }
        bool EmailCheck(string EmailString)
        {
            foreach(string bufs in Mails)
            {
                if (bufs == EmailString)
                    return true;
            }
            return false;
        }
        private bool ListCheck(string turgetString,List<string> bul)
        {

            foreach(var bufs in bul)
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
        private void FindPlainTextInMessage(OpenPop.Mime.Message message)
        {
            MessagePart plainText = message.FindFirstPlainTextVersion();
            if (plainText != null)
            {
                // Save the plain text to a file, database or anything you like
                plainText.Save(new FileInfo(@"Texter.txt"));
                FileStream reder = new FileStream(@"Texter.txt", FileMode.Open);
                StreamReader reader = new StreamReader(reder);
                string bufs = reader.ReadToEnd();
                BufferMailText = bufs;
                reder.Close();
                File.Delete(@"Texter.txt");
                FlowDocument document = new FlowDocument();
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(new Bold(new Run(bufs)));
                document.Blocks.Add(paragraph);
                TextMessag.Document = document;
            }
            else
            {
                plainText = message.FindFirstHtmlVersion();
                plainText.Save(new FileInfo(@"Texter.txt"));
                FileStream reder = new FileStream(@"Texter.txt", FileMode.Open);
                StreamReader reader = new StreamReader(reder);
                string bufs = reader.ReadToEnd();
                bufs = bufs.Replace("<br>", "\n");
                bufs = bufs.Replace("<HTML>", ""); bufs = bufs.Replace("</HTML>", "");
                bufs = bufs.Replace("<BODY>", ""); bufs = bufs.Replace("</BODY>", "");
                bufs = bufs.Replace("<p>", ""); bufs = bufs.Replace("</p>", "");
                bufs = bufs.Replace("<div>", ""); bufs = bufs.Replace("</div>", "");
                BufferMailText = bufs;
                reder.Close();
                File.Delete(@"Texter.txt");
                FlowDocument document = new FlowDocument();
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(new Bold(new Run(bufs)));
                document.Blocks.Add(paragraph);
                TextMessag.Document = document;
            }

            
            
        }

        string checkDataList()
        {
            string bus="";

            int i = 0;

            foreach( var bufsl in listWithDataToProtect)
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
                if((bufs[0]==selectedEmailAdress)&&(bufs[1]==bufhe)&&(bufData==bufs[2]))
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
        void getEmail()
        {
            try
            {
                string[] words = login.Split(new char[] { '@' });
                string pop3 = "pop." + words[1];
                using (Pop3Client client = new Pop3Client())
                {
                    client.Connect(pop3, 995, true);
                    client.Authenticate(login, pass);
                    int messageCount = client.GetMessageCount();
                    Message a = client.GetMessage(messageCount - checkList());
                    FindPlainTextInMessage(a);
                    System.Threading.Thread.Sleep(10);



                }
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Вернитесь в меню!");
                AllSender.IsEnabled = Filter.IsEnabled = Group.IsEnabled = false;
            }

        }

        private void AllSender_MouseEnter(object sender, MouseEventArgs e)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////
            if (AllSender.SelectedIndex >= 0)
            {
                switch (listType)
                {
                    case 0:                       
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
                       
                        getEmail();
                        AllSender.Visibility = Visibility.Visible;
                        break;
                }
                ;
            }
            
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            var mm = new MainMenu(pass,login);
            CloseCheck = true;
            mm.Show();
            this.Close();
        }


        private void BackCon_Click(object sender, RoutedEventArgs e)
        {
            listType--;
            if(listType==1)
                listEmailAdresses();
          //  addListHeaderEmail();
            if(listType == 0)
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

            var sss = Decripter.IsChecked;
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
            string bufsMail= PasswordMailCheck(bufs);
                if(bufsMail!="")
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
                 if (Group.SelectedIndex >= 0)
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
            if (Group.SelectedIndex > -1)
            {
                int i = 0;
                foreach (string bufs in NamesAdr)
                {
                    if ((bufs.IndexOf(sometext) > -1) && (NamesGroup[i] == Group.SelectedItem.ToString()))
                        bufList.Add(bufs);
                    i++;
                }
            }
            return bufList;
        }

        private void Decripter_IsCheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
