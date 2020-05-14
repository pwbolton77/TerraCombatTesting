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
using System.ComponentModel;
using TerraCombatTesting.ViewModel;

namespace TerraCombatTesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MainViewModel = new MainViewModel();
            InitializeComponent();
            DataContext = MainViewModel;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //_viewModel.OnClosing(e);
        }

        public MainViewModel MainViewModel { get; set; }

        private async void ButonRunTrials(object sender, RoutedEventArgs e)
        {
            // Async WPF at: https://stackoverflow.com/questions/27089263/how-to-run-and-interact-with-an-async-task-from-a-wpf-gui/27089652
            MessageBox.Show("Running ...");

            //queue a task to run on threadpool
            Task task = Task.Run(() => ExecuteLongProcedure(this, MainViewModel.OffenseRating, MainViewModel.DefenseRating, MainViewModel.NumTrials) );

            // ExecuteLongProcedure is now running asynchronously

            //wait for it to end without blocking the main thread
            await task;

            MessageBox.Show("Done!");
        }

        void ExecuteLongProcedure(MainWindow gui, int param1, int param2, int param3)
        {
            System.Threading.Thread.Sleep(3000);
        }
    }

}
