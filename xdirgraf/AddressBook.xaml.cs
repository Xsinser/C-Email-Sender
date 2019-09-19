using System;

using Microsoft.Win32;
using System.IO;
using System.Windows.Resources;
using System.Drawing;

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
using System.Data.SQLite;
namespace xdirgraf
{
    /// <summary>
    /// Логика взаимодействия для AddressBook.xaml
    /// </summary>
    public partial class AddressBook : MetroWindow
    {

        private List<string> Mails=new List<string>();
        private List<string> Group = new List<string>();
        private List<string> NamesAdr = new List<string>();
        private int id;
        private string login ="",pass="";
        bool CloseCheck = false;
        public AddressBook(string Login, string Pass)
        {
            InitializeComponent();
            login = Login;
            pass = Pass;
            id = loginCheck();
            updateAll();
            Emails.ItemsSource = Mails;

        }

        public AddressBook(string targetEmail,string Login,string Pass)
        {
            InitializeComponent();
            try
            {
                login = Login;
                pass = Pass;
                id = loginCheck();
                updateAll();
                Mails.Add(targetEmail);
                Emails.ItemsSource = Mails;
                CheckPol.IsChecked = true;
                CheckPol.IsEnabled = false;
          

            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Вернитесь в меню!");
                BClick.IsEnabled = AddGroup.IsEnabled = DelMail.IsEnabled = AddMail.IsEnabled = CheckPol.IsEnabled = false;
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
                    return Convert.ToInt16(read[0]);
                }
            }
            con.Close();
            return -1;
        }

        void updateAll()
        {
            Emails.Text = "";
            Emails.ItemsSource = null;
            GroupCombo.ItemsSource = null;
            GroupCombo.Text = null;
            ListV.ItemsSource = null;
            NamesCombo.Text = "";
            Mails.Clear(); Group.Clear(); NamesAdr.Clear();
            GetGroups();
            GroupCombo.ItemsSource = Group;
            //  Email.ItemsSource = listv;
            GetNames();
            ListV.ItemsSource = NamesAdr;
        }
        // загрузка групп из бд
        void GetGroups()
        {
            var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
            con.Open();
            var com = new SQLiteCommand("Select * from type_address", con);
            var read = com.ExecuteReader();
          
            while (read.Read())
            {
                Group.Add(read[0].ToString());
            }
            con.Close();
        }
        // загрузка адресатов
        void GetNames()
        {
            var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
            con.Open();
            var com = new SQLiteCommand("Select * from Address_Data where member='" + id + "'", con);
            var read = com.ExecuteReader();

            while (read.Read())
            {
                NamesAdr.Add(read[1].ToString());
            }
            con.Close();
        }
        // изменение поиска адресатов
        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(bool)CheckPol.IsChecked)
            {
                ListV.Visibility = Visibility;
                //     NamesCombo.IsDropDownOpen = true;
                //    var cmbTextBox = (TextBox)NamesCombo.Template.FindName("PART_EditableTextBox", NamesCombo);
                //      cmbTextBox.CaretIndex = NamesCombo.Text.Length;
                ListV.ItemsSource = FilterBox(NamesCombo.Text);
            }
            else
                ListV.Visibility = Visibility.Hidden;
        }
        // фильтр листбокса для поиска
        List<string> FilterBox(string sometext)
        {
            List<string> bufList = new List<string>();
            foreach(string bufs in NamesAdr)
            {
                if (bufs.IndexOf(sometext) > -1)
                    bufList.Add(bufs);
            }
            return bufList;
        }
        // удалить
        private void TextTags_TextChanged(object sender, TextChangedEventArgs e)
        {


        }
        // не работает
        private void Email_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool a = NamesCombo.Focusable;
            if (!NamesCombo.Focusable)
                ListV.Visibility = Visibility.Hidden;
        }
        // изменение типа
        private void CheckPol_Checked(object sender, RoutedEventArgs e)
        {
            BClick.Content = "Добавление нового контакта";
        }

        private void CheckPol_Unchecked(object sender, RoutedEventArgs e)
        {
            BClick.Content = "Сохранить изменения";
        }
        // получение всех писем выбраного контакта
        void getMails(string TargetStr)
        {
            Mails.Clear();
            string[] words = TargetStr.Split(new char[] { ' ' });
            for(int i = 0; i<words.Length;i++)
            {
                Mails.Add(words[i]);
            }
        }
        void GetPhoto(byte[] massb )
        {
          ImageBox.Source= SupClass.ConvertByteArrayToBitmapImage(massb);
        }

        private void NamesCombo_Selected(object sender, RoutedEventArgs e)
        {try
            {
                if (ListV.SelectedIndex != -1)
                {
                    NamesCombo.Text = ListV.SelectedItem.ToString();
                    ListV.Visibility = Visibility.Hidden;

                    // int indexSel = NamesAdr.IndexOf( NamesCombo.Text);
                    var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
                    con.Open();
                    var com = new SQLiteCommand("Select * from Address_Data where Name='" + NamesCombo.Text + "' and member='" + id + "'", con);
                    var read = com.ExecuteReader();

                    read.Read();
                    {
                        id = Convert.ToInt16(read[0]);
                        GroupCombo.Text = read[4].ToString();
                        getMails(read[2].ToString());
                        Emails.ItemsSource = null;
                        Emails.ItemsSource = Mails;
                        GetPhoto((byte[])read[3]);
                    }
                    con.Close();
                }
            }
            catch
            {

            }
        }

        private void DelMail_Click(object sender, RoutedEventArgs e)
        {
          Mails.Remove(Emails.Text);
          Emails.ItemsSource = null;
          Emails.ItemsSource = Mails;
        }

        private void AddMail_Click(object sender, RoutedEventArgs e)
        {
            Mails.Add(Emails.Text);
            Emails.ItemsSource = null;
            Emails.ItemsSource = Mails;
        }
        //картинка
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImageBox.Source = new BitmapImage(new Uri( openFileDialog.FileName));
                
            }
        }

        string MailsConvert()
        {
            string bufs = "";
            foreach(string bufLs in Mails)
            {
                bufs += (bufLs + " ");
            }
            return bufs.Trim();
        }

        void AddNewAddress()
        {
         
            var img=       SupClass.ConvertBitmapSourceToByteArray(new PngBitmapEncoder(), ImageBox.Source);
            GroupInsertAndAdd();
            var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
            con.Open();
            var com = new SQLiteCommand("insert into Address_Data (Name, amails, photo, type, member) values ('" + NamesCombo.Text + "', '"+
            MailsConvert() + "', @imag, '"+ GroupCombo.Text+"','"+id+"')", con);
            com.Parameters.Add(new SQLiteParameter("imag",img));
            com.ExecuteScalar();
            con.Close();
            Group.Clear();
            GetGroups();
        }

        void UpdateAddress()
        {
           
                var img = SupClass.ConvertBitmapSourceToByteArray(new PngBitmapEncoder(), ImageBox.Source);
                GroupInsertAndAdd();
                var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
                con.Open();
            try
            {
                var com = new SQLiteCommand("UPDATE Address_Data  SET  amails='" +
                //Name='" + NamesCombo.Text"

                MailsConvert() + "', photo= @imag, type = '" + GroupCombo.Text + "', member ='" + id + "' where Name='" + NamesCombo.Text + "'", con);
                com.Parameters.Add(new SQLiteParameter("imag", img));
                com.ExecuteScalar();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка!");
            }
            con.Close();
            Group.Clear();
            GetGroups();
        }

        private void BClick_Click(object sender, RoutedEventArgs e)
        {
           
            if(Convert.ToBoolean(CheckPol.IsChecked))
            {
                AddNewAddress();
            }
            else
            {
                UpdateAddress();
            }
            updateAll();
        }
        bool ChekGroups(string sForCheck)
        {
            foreach(string bufs in Group)
            {
                if(bufs== sForCheck)
                {
                    return false;
                }
            }
            return true;
        }

        void GroupInsertAndAdd()
        {
            if (ChekGroups(GroupCombo.Text))
            {
                var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
                con.Open();
                var com = new SQLiteCommand("insert into type_address (type_name) values ('" + GroupCombo.Text + "')", con);
                com.ExecuteScalar();
                con.Close();
                Group.Clear();
                GetGroups();
            }
        }
        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            GroupInsertAndAdd();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CloseCheck)
                System.Windows.Application.Current.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var mm = new MainMenu(pass, login);
            mm.Show();
            CloseCheck = true;
            this.Close();          

        }

        private void Email_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListV.Visibility = Visibility.Hidden;
        }
    }
}
