using Base;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace JournalManagmentStudio.Project.Project
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                DataContext = new ViewModel.MainViewModel(args[1]);
            }
            else
                DataContext = new ViewModel.MainViewModel();
        }
    }
}
