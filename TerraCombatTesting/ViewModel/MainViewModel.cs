using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraCombatTesting.ViewModel
{
    public class MainViewModel : Observable
    {
        private int _offenseRating;

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

        private int _defenseRating;
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

        private int _numTrials;
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
    }
}
