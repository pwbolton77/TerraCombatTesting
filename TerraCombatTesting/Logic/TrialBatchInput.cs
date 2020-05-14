using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraCombatTesting.Logic
{
    public class TrialBatchInput
    {
        public TrialBatchInput(int offense_rating, int defense_rating, int num_trials)
        {
            OffenseRating = offense_rating;
            DefenseRating = defense_rating;
            NumTrials = num_trials;
        }
        public int OffenseRating { get; set; }
        public int DefenseRating { get; set; }
        public int NumTrials { get; set; }
    }
}
