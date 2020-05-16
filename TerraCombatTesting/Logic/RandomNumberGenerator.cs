using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraCombatTesting.Logic
{
    class RandomNumberGenerator
    {

        private Random _rnd;
        static Object _randLock;
        public RandomNumberGenerator(int seed)
        {
            _rnd = new Random(seed);
            _randLock = new Object();
        }

        public int RollDice(int num_sides, int num_die = 1)
        {
            int result = 0;
            lock (_randLock)
            {
                for (var die = 0; die < num_die; ++die)
                    result += _rnd.Next(1, num_sides+1);
            }
            return result;
        }
    }
}

