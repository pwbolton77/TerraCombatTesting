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
        private RandomNumberGenerator _rng = new RandomNumberGenerator( 1 /* starting seed */);

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

        private void EnableTrialEntryInput(bool is_enabled)
        {
            OffenseRatingTextBox.IsEnabled = is_enabled;
            DefenseRatingTextBox.IsEnabled = is_enabled;
            TrialNumTextBox.IsEnabled = is_enabled;
            RunTrialsButton.IsEnabled = is_enabled;
        }

        private async void RunTrialsButtonClicked(object sender, RoutedEventArgs e)
        {
            // Async WPF at: https://stackoverflow.com/questions/27089263/how-to-run-and-interact-with-an-async-task-from-a-wpf-gui/27089652
            EnableTrialEntryInput(false /* disable */);

            TrialBatchInput input = new TrialBatchInput(MainViewModel.OffenseRating, MainViewModel.DefenseRating, MainViewModel.NumTrials);

            //queue a task to run on threadpool
            Task<TrialBatchResult> task = Task.Run(() => ExecuteTrialBatch(this, MainViewModel.OffenseRating, MainViewModel.DefenseRating, MainViewModel.NumTrials));

            // ExecuteLongProcedure is now running asynchronously

            //wait for it to end without blocking the main thread
            var result = await task;

            double hitPercent = 0.0;
            double criticalHitPercent = 0.0;

            if (result.NumTrials > 0)
            {
                hitPercent = (100.0 * result.Hits) / result.NumTrials;
                criticalHitPercent = (100.0 * result.CriticalHits) / result.NumTrials;
            }

            MainViewModel.ResultsLog = 
                $"Hits: {result.Hits} ({hitPercent :F2}%)" + 
                $"  Critical Hits: {result.CriticalHits} ({criticalHitPercent :F2}%)" + 
                $"  OR: {result.OffenseRating}  DR: {result.DefenseRating}  Num Trials: {result.NumTrials}\n" 
                + MainViewModel.ResultsLog;
            // MessageBox.Show(msg);

            EnableTrialEntryInput(true /* enable */);
        }



        TrialBatchResult ExecuteTrialBatch(MainWindow gui, int offenseRating, int defenseRating, int numTrials)
        {
            // System.Threading.Thread.Sleep(1);   // 1000 is one second
            int hits = 0;
            int critical_hits = 0;

            for (int trial = 0; trial < numTrials; ++trial)
            {
                bool hit = false;
                bool critical_hit = false;
                hit = HitRoll(offenseRating, defenseRating, out critical_hit);

                if (hit) ++hits;
                if (critical_hit) ++critical_hits;
            }

            return new TrialBatchResult(offenseRating, defenseRating, numTrials, hits, critical_hits);
        }

        private bool HitRoll(int offenseRating, int defenseRating, out bool criticalHit)
        {
            // TODO: JOSH - Rewrite this function as you see fit.
            int deltaRating = offenseRating - defenseRating;

            int roll = _rng.RollDice(100);

            bool hit_result = false;
            if (roll > 50 - deltaRating)
                hit_result = true;

            if (roll > 95)
                criticalHit = true;
            else
                criticalHit = false;

            return hit_result;
        }
    }

}
