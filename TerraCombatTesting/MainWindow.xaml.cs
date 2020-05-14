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

        private void ButonRunTrials(object sender, RoutedEventArgs e)
        {
            string or = MainViewModel.OffenseRating.ToString();
            string dr = MainViewModel.DefenseRating.ToString();
            string num_trials = MainViewModel.NumTrials.ToString();

            string msg = $"OR= {or}  DR={dr}  trials={num_trials}";

            MessageBox.Show(msg);
        }
    }
}
