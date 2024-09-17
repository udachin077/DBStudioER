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

namespace JournalManagmentStudio.Project.View
{
    /// <summary>
    /// Логика взаимодействия для ASUDScadaTableView.xaml
    /// </summary>
    public partial class ASUDScadaTableView : UserControl
    {
        public ASUDScadaTableView()
        {
            InitializeComponent();
        }

        private char[] symbol = new char[] { ',', '.', '\'', '\\', '/', '\"', ';' };

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (char c in symbol)
            {
                ((TextBox)sender).Text = ((TextBox)sender).Text.Replace(c, ':');
            }
            ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length;
        }
    }
}
