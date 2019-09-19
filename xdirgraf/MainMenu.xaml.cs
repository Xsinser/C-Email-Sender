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
using System.Data.SQLite;
namespace xdirgraf
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : MetroWindow
    {
    private     string pass, login;
      bool  CloseCheck = false;
        public MainMenu()
        {
            InitializeComponent();
        }

        public MainMenu(string Pass, string Login)
        {
            InitializeComponent();
            pass = Pass;
            login = Login;
            var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
            con.Open();
            if (!loginCheck())
            {
                var com = new SQLiteCommand("insert into Members (mails) values ('" + login + "')", con);
                com.ExecuteScalar();

            }
            con.Close();
          
        }
         
        bool loginCheck()
        {
            var con = new SQLiteConnection("Data Source=Address.db; Password=23;");
            con.Open();
            var com = new SQLiteCommand("Select * from Members", con);
            var read = com.ExecuteReader();
            bool bufB=false;
            while(read.Read())
            {
                if (read[1].ToString() == login)
                {

                    bufB = true;
                    break;
                }
            }
            con.Close();
            return bufB;


        }
        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {
            var sm = new SendEmail(pass, login);
            sm.Show();
            CloseCheck = true;
            this.Close();

        }
        private void Button_Click_Read(object sender, RoutedEventArgs e)
        {
            var sm = new Load(pass, login);
            sm.Show();
            CloseCheck = true;
            this.Close();
    
        }
        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            var sm =SupClass.ToMainWindow;
            sm.Show();
            CloseCheck = true;
            this.Close();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sm = new AddressBook(login, pass);
            sm.Show();
            CloseCheck = true;
            this.Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!CloseCheck)
            System.Windows.Application.Current.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var sm = new LoadImap(pass, login);
            sm.Show();
            CloseCheck = true;
            this.Close();
        }

        private void AboutProgramm_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Версия программы: 1.0.0 \nАдрес почты разработчика: xsinser2@mail.ru ", "О программе");
        }

        private void Waring_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"Help.docx");

        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
