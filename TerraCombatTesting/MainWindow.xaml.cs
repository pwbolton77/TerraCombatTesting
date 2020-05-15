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
using TerraCombatTesting.Logic;

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
            MainViewModel.ResultsLog = "Hello!";
            InitializeComponent();
            DataContext = MainViewModel;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //_viewModel.OnClosing(e);
        }

        public MainViewModel MainViewModel { get; set; }

        private void EnableTrialEntryInput(bool is_enabled)
        {
            OffenseRatingTextBox.IsEnabled = is_enabled;
            DefenseRatingTextBox.IsEnabled = is_enabled;
            TrialNumTextBox.IsEnabled = is_enabled;
            RunTrialsButton.IsEnabled = is_enabled;
        }

        private async void RunTrialsButonClicked(object sender, RoutedEventArgs e)
        {
            // Async WPF at: https://stackoverflow.com/questions/27089263/how-to-run-and-interact-with-an-async-task-from-a-wpf-gui/27089652
            EnableTrialEntryInput(false /* disable */);

            TrialBatchInput input = new TrialBatchInput(MainViewModel.OffenseRating, MainViewModel.DefenseRating, MainViewModel.NumTrials);

            //queue a task to run on threadpool
            Task<TrialBatchResult> task = Task.Run(() => ExecuteLongProcedure(this, MainViewModel.OffenseRating, MainViewModel.DefenseRating, MainViewModel.NumTrials));

            // ExecuteLongProcedure is now running asynchronously

            //wait for it to end without blocking the main thread
            var result = await task;

            MainViewModel.ResultsLog = 
                $"Hits: {result.Hits}  Critical Hits: {result.CriticalHits}  OR: {result.OffenseRating}  DR: {result.DefenseRating}  Num Trials: {result.NumTrials}\n" 
                + MainViewModel.ResultsLog;
            // MessageBox.Show(msg);

            EnableTrialEntryInput(true /* enable */);
        }

        TrialBatchResult ExecuteLongProcedure(MainWindow gui, int offenseRating, int defenseRating, int numTrials)
        {
            System.Threading.Thread.Sleep(500);
            return new TrialBatchResult(offenseRating, defenseRating, numTrials, 100, 5);
        }
    }

}
