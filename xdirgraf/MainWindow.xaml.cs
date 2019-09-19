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
using MahApps.Metro.Controls;
using OpenPop.Mime;
using OpenPop.Mime.Header;
using OpenPop.Pop3.Exceptions;
using OpenPop.Common;
using OpenPop.Common.Logging;
using OpenPop.Mime.Decode;
using OpenPop.Pop3;

namespace xdirgraf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
        string[] words=    EmailBox.Text.Split(new char[] { '@' });
          string  pop = "pop." + words[1];


            if (logintest(pop,EmailBox.Text, PasswordBox.Password))
            {
                SupClass.ToMainWindow = this;
                this.Hide();
                var fr = new MainMenu(PasswordBox.Password, EmailBox.Text);

                fr.Show();
            }
        }

        private bool logintest(string popserver, string login, string pass)
        {
            using (Pop3Client client = new Pop3Client())
            {


                try
                {
                    client.Connect(popserver, 995, true);
                    client.Authenticate(login, pass);
                    
                    return true;
                }
                catch (Exception ex)
                {
                    if ( "Server did not accept user credentials"   == ex.Message.ToString())
                    {
                        MessageBox.Show("Логин или пароль введены неверно!","Ошибка Авторизации!" );
                    }
                     return false;
                }


            }
        }
    }
}
