using System.Windows.Controls;

namespace JournalManagmentStudio.Project.View
{
    /// <summary>
    /// Логика взаимодействия для HorizonTable.xaml
    /// </summary>
    public partial class HorizonTableView : UserControl
    {
        public HorizonTableView()
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
