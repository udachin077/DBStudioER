﻿using JournalManagmentStudio.Journal.ViewModel;
using System.Windows;

namespace JournalManagmentStudio.Journal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
