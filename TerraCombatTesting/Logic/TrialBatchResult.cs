using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TerraCombatTesting.Logic
{
    public class TrialBatchResult : TrialBatchInput
    {
        public TrialBatchResult(int offense_rating, int defense_rating, int num_trials, int hits, int critical_hits)
            : base(offense_rating, defense_rating, num_trials)
        {
            Hits = hits;
            CriticalHits = critical_hits;
        }

        public int Hits { get; set; }
        public int CriticalHits { get; set; }
    }
}
