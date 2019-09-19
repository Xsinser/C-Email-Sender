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
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Net.Mail;
using XsenoCode;
using System.Data.SQLite;

namespace xdirgraf
{
    public partial class SendEmail : MetroWindow
    {
        private string pass, login,passEmail="";

        private List<string> listAdress=new List<string>();
        private List<string> Mails = new List<string>();
        private List<string> GroupBd = new List<string>();
        private List<string> NamesAdr = new List<string>();
        private List<string> NamesGroup = new List<string>();
        private int id;
        bool CloseCheck = false;
        public SendEmail()
        {
            InitializeComponent();
        }
        public SendEmail(string Pass, string Login)
        {
  
                InitializeComponent();
            try
            {
                pass = Pass;
                login = Login;
                id = loginCheck();
                GetNames();
                GetGroups();
                Group.ItemsSource = GroupBd;
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Вернитесь в меню!");
                SendB.IsEnabled = false;
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
            var com = new SQLiteCommand("Select * from Address_Data where member='"+id+"'", con);
            var read = com.ExecuteReader();

            while (read.Read())
            {
                NamesAdr.Add(read[1].ToString());
                NamesGroup.Add(read[4].ToString());
            }
            con.Close();
        }


        bool adressListChek()
        {
            foreach(var bufs in listAdress)
            {
                if (Email.Text == bufs)
                    return false;
            }
            return true;
        }


        private void SendB_Click(object sender, RoutedEventArgs e)
        {
            var textRange = new TextRange(TextMessag.Document.ContentStart, TextMessag.Document.ContentEnd);
            if ((Email.Text.Length>0)&&(textRange.Text.Length>0)&&(Header.Text.Length>0))
            {
                MailAddress from = new MailAddress(login, "");
                // кому отправляем
                MailAddress to = new MailAddress(Email.Text);
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = Header.Text;
               
                if (Convert.ToBoolean(Encripter.IsChecked))
                {
                    XenoCode a = new XenoCode();

                    m.Body = passEmail + a.RSA_encryption(textRange.Text, login, Email.Text) + "&";

                }
                else

                { m.Body = textRange.Text; }
                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                string[] words = login.Split(new char[] { '@' });
                string smt = "smtp." + words[1];
                SmtpClient smtp = new SmtpClient(smt, 587);
                // логин и пароль
                smtp.Credentials = new NetworkCredential(login, pass);
                smtp.EnableSsl = true;

                try
                {
                    smtp.Send(m);
                    FlowDocument document = new FlowDocument();
                    Paragraph paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Bold(new Run("")));
                    document.Blocks.Add(paragraph);
                    TextMessag.Document = document;

                    if (Convert.ToBoolean(CheckSave.IsChecked))
                    {
                        GoToAddress WindowAddres = new GoToAddress();

                        if (WindowAddres.ShowDialog() == true)
                        {
                            AddressBook ad = new AddressBook(Email.Text, login, pass);
                            ad.Show();
                            CloseCheck = true;
                            this.Close();
                        }
                    }
                    Email.Text = Header.Text = "";
                }
                catch
                {
                    MessageBox.Show("Письмо не отправлено!","Ошибка");
                }

            }
            else
            {
                MessageBox.Show("Не все поля заполнены!","Внимание!");
            }

            

        }

        private void CheckMailPass_Checked(object sender, RoutedEventArgs e)
        {

            if ((Convert.ToBoolean(CheckMailPass.IsChecked))&&(Email.Text!=""))
            {

                PasswordWindow passwordWindow = new PasswordWindow();

                if (passwordWindow.ShowDialog() == true)
                {
                    XenoCode a = new XenoCode();
                    passEmail = a.RSA_encryption(passwordWindow.Password, login, Email.Text) + "!";
                    Email.IsEnabled = false;
                    ListV.Visibility = Visibility.Hidden;
                }
                else
                {
                    CheckMailPass.IsChecked = false;
                    Email.IsEnabled = !false;
                }

            }

        }

        private void CheckSave_Checked(object sender, RoutedEventArgs e)
        {
            Group.IsEnabled = Names.IsEnabled = false;
             Names.Text=Group.Text = "";
            ImageBox.Visibility = Visibility.Hidden;
            Email.IsEditable = true;
        }

        private void CheckSave_Unchecked(object sender, RoutedEventArgs e)
        {
            Group.IsEnabled = Names.IsEnabled = true;
            ImageBox.Visibility = Visibility.Visible;
            Email.IsEditable = false;
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

        void GetPhoto(byte[] massb)
        {
            ImageBox.Source = SupClass.ConvertByteArrayToBitmapImage(massb);
        }
        private void ListV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListV.SelectedIndex != -1)
                {
                    Names.Text = ListV.SelectedItem.ToString();
                    ListV.Visibility = Visibility.Hidden;
                    Email.ItemsSource = null;
                    var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
                    con.Open();
                    var com = new SQLiteCommand("Select * from Address_Data where Name='" + Names.Text + "' and member='" + id + "'", con);
                    var read = com.ExecuteReader();

                    read.Read();
                    {

                        getMails(read[2].ToString());
                        Email.ItemsSource = Mails;
                        GetPhoto((byte[])read[3]);
                    }
                    con.Close();
                }
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Вернитесь в меню!");
                SendB.IsEnabled = false;
            }
        }

        private void Names_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListV.Visibility = Visibility;
            if(Group.Text=="")
            ListV.ItemsSource = FilterBox(Names.Text);
            else
                 if (Group.SelectedIndex >= 0)
                ListV.ItemsSource = FilterBoxWithGroup(Names.Text);
        }
        // фильтр листбокса для поиска
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
                if ((bufs.IndexOf(sometext) > -1)&&(NamesGroup[i]==Group.SelectedItem.ToString()))
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
            ListV.Visibility = Visibility.Visible;
            if (Names.Text == "")
                ListV.ItemsSource = FilterGroup(Names.Text);
            else
                 if (Group.SelectedIndex >= 0)
                ListV.ItemsSource = FilterBoxWithGroup(Names.Text);
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

        private void Names_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListV.Visibility = Visibility.Visible;
        }

        private void Email_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListV.Visibility = Visibility.Hidden;
        }

        private void CheckMailPass_Unchecked(object sender, RoutedEventArgs e)
        {
            Email.IsEnabled = !false;
            passEmail = "";
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            var mm = new MainMenu(pass,login);
            mm.Show();
            CloseCheck = true;
            this.Close();
        }
    }
}
