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
namespace xdirgraf
{
    /// <summary>
    /// Логика взаимодействия для PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : MetroWindow
    {
        private string SavePass;
        public PasswordWindow()
        {
            InitializeComponent();
            passwordBox.Focus();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            SavePass= passwordBox.Text;
            this.DialogResult = true;
        }

        public string Password
        {
            get { return SavePass; }
        }
    }
}
