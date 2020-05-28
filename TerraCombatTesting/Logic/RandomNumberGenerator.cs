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


        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
        /// </summary>
        /// <returns>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</returns>
        public double RollDouble()
        {
            double result = 0.0;
            lock (_randLock)
            {
                result = _rnd.NextDouble();
            }
            return result;
        }
    }
}

