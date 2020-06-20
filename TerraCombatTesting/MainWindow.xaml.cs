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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TerraCombatTesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RandomNumberDiceGenerator _rng = new RandomNumberDiceGenerator(1 /* starting seed */);
        private DiceEvaluator _dice_evaluator;

        public MainWindow()
        {
            MainViewModel = new MainViewModel();
            _dice_evaluator = new DiceEvaluator(_rng);

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
            ClearResultsButton.IsEnabled = is_enabled;
        }

        private async void RunTrialsButtonClicked(object sender, RoutedEventArgs e)
        {
            PrintQuantiles(); //Josh's Function to value check hit rating function -JB 5/25/20
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
                $"Hits: {result.Hits} ({hitPercent:F2}%)" +
                $"  Critical Hits: {result.CriticalHits} ({criticalHitPercent:F2}%)" +
                $"  OR: {result.OffenseRating}  DR: {result.DefenseRating}  Num Trials: {result.NumTrials} Combat Residual: {Math.Abs(result.OffenseRating - result.DefenseRating)}\n"
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

            double critChance = 5; // (in %%%%)
            int ratingResidual = offenseRating - defenseRating; // The difference between Attack and Defense (Positive = attackeradv| Negative = defadv)
            /* Rolling to determine Hit Chance */
            double hit_chance = HitProbFunc(ratingResidual);
            // PrintQuantiles();

            criticalHit = false; // reset criticalHit indicator
            bool hit_result; // reseet hit_result indicator

            int roll = _rng.RollDice(100); //the player's 'roll to hit'
            if (roll <= hit_chance)
            {
                hit_result = true; // successful hit from attacker
                if (roll <= critChance) criticalHit = true; // Checks to see if the roll was a crit simply if the roll is within the proportion (crit%/100) of the roll
            }
            else
            {
                hit_result = false;
            }
            return hit_result;
        }


        private double HitProbFunc(double x)
        {
            // My graph this is based off of https://www.desmos.com/calculator/gtm3i8vcbf -JB

            /*  FUNCTION COEFFICIENTS */
            double max = 50.00; //this is max percent chance of winning, when OC=AC, (always 50% if OC=AC)
            double slope = 0.08; //Changes the effect of the diminishing returns within the graph
            double midp = 42.5; //Changes the midpoint of the graph

            double a_coef = 0.5; // Domain control (X)
            double b_coef = 2; // Range control (Y)
            double c_coef = 42.5; // Intercept Control
            // x = armor residual (In this case)

            double pwr = -1 * slope * ((a_coef * x + c_coef) - midp); // seperating defining the power term of the exp function
            double hit_prob = b_coef * (max * (1 / (1 + Math.Exp(pwr))));
            if (hit_prob < 5.0) hit_prob = 5.0;
            if (hit_prob > 95.0) hit_prob = 95.0;// enables cut off values of 95% and 5% as well as normalizes to PDF standards

            return (hit_prob); // Attacker stronger
        }

        // Used to just show upper and lower quantiles of the hitProbFunction
        private void PrintQuantiles()
        {
            Debug.WriteLine("Armor Residual Distribution Quantiles");
            for (int i = 0; i <= 10; i++)
            {
                Debug.WriteLine("------------x-------------");
                Debug.WriteLine("adv Residual: " + i * 10 + ": " + HitProbFunc(i * 10));
                Debug.WriteLine("dis Residual: " + i * 10 + ": " + HitProbFunc(i * 10));
            }
        }

        private void ClearResultsButtonClicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.ResultsLog = "";
        }

        private void DamageExpressionTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Do evaluation when Enter key is pressed
            if (e.Key == Key.Return)
            {
                bool eval_okay = _dice_evaluator.Evaluate(MainViewModel.DamageExpression, out int value);

                if (eval_okay)
                    MainViewModel.ResultsLog = MainViewModel.DamageExpression + " |  value: " +  value.ToString() + "\n" + MainViewModel.ResultsLog;
                else
                    MainViewModel.ResultsLog = MainViewModel.DamageExpression + " |  ERROR!" + "\n" + MainViewModel.ResultsLog;
            }
        }
    }
}

