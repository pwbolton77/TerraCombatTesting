using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraCombatTesting.ViewModel
{
    public class MainViewModel : Observable
    {
        private int _offenseRating = 50;

        public int OffenseRating
        {
            get { return _offenseRating; }
            set 
            { 
                if (value != _offenseRating)
                {
                    _offenseRating = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _defenseRating = 50;
        public int DefenseRating
        {
            get { return _defenseRating; }
            set 
            { 
                if (value != _defenseRating)
                {
                    _defenseRating = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _numTrials = 10000000;
        public int NumTrials
        {
            get { return _numTrials; }
            set 
            { 
                if (value != _numTrials)
                {
                    _numTrials= value;
                    OnPropertyChanged();
                }
            }
        }

        private string _damageExpression;

        public string DamageExpression
        {
            get { return _damageExpression; }
            set 
            { 
                if (value != _damageExpression)
                {
                    _damageExpression = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _resultsLog;
        public string ResultsLog 
        {
            get { return _resultsLog; }
            set
            {
                if (value != _resultsLog)
                {
                    _resultsLog = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
