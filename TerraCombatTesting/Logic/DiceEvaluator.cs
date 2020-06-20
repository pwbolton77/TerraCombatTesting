using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerraCombatTesting.Logic
{
    class DiceEvaluator
    {
        private static readonly Dictionary<string, int> _precedence = new Dictionary<string, int> { { "(", 0 }, {"d", 1}, { "*", 1 }, { "/", 1 }, { "+", 2 }, { "-", 2 }, { ")", 3 } };

        /// <summary>
        /// Constructor that relies on a provided RandomNumberDiceGenerator
        /// </summary>
        /// <param name="rng"></param>
        public DiceEvaluator(RandomNumberDiceGenerator rng)
        {
            RandomNumberDiceGenerator = rng;
        }

        /// <summary>
        /// Constructor that creates its own RandomNumberDiceGenerator with a random seed.
        /// </summary>
        public DiceEvaluator()
        {
            RandomNumberDiceGenerator = new RandomNumberDiceGenerator();
        }    

        public RandomNumberDiceGenerator RandomNumberDiceGenerator { get; set; }

        /// <summary>
        /// Take an input string like "2d6+4", parse it as infix notation, and evaluate it including rolling dice.
        /// The parsing supports parenthis, '+', '*', and the 'd" operators wher
        ///     'd' indicates roll dice (e.g 2d8 means roll 2 eight sided dice and sum).
        /// Negative numbers and the '-' subtraction operator is NOT supported.
        /// </summary>
        /// <param name="input_string"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Evaluate(string input_string, out int value)
        {
            bool eval_okay = true;
            try
            {
                // https://stackoverflow.com/questions/4680128/split-a-string-with-delimiters-but-keep-the-delimiters-in-the-result-in-c-sharp
                // Example: Regex.Split("10E-02*x+sin(x)^2", @"([*()\^\/]|(?<!E)[\+\-])")
                const string pattern = @"([+*d\(\)])";

                string[] substrings = Regex.Split(input_string, pattern);

                List<string> infix = substrings.ToList();

                var operatorstack = new Stack<string>();
                var operandstack = new Stack<int>();

                // Wrap with parenthisis 
                infix.Insert(0, "(");
                infix.Add(")");

                foreach (var token in infix)
                {
                    if (token == "")
                    { 
                        // Ignore - do nothing
                    }
                    else if (_precedence.ContainsKey(token)) // If token in an operator
                    {
                        var keepLooping = true;
                        while (keepLooping && operatorstack.Count > 0 && _precedence[token] > _precedence[operatorstack.Peek()])
                        {
                            switch (operatorstack.Peek())
                            {
                                case "d":
                                    var roll = RandomNumberDiceGenerator.RollDice(operandstack.Pop(), operandstack.Pop());
                                    Debug.WriteLine($"Roll: {roll}");
                                    operandstack.Push(roll);
                                    break;
                                case "+":
                                    operandstack.Push(operandstack.Pop() + operandstack.Pop());
                                    break;
                                case "-":
                                    operandstack.Push(-operandstack.Pop() + operandstack.Pop());
                                    break;
                                case "*":
                                    operandstack.Push(operandstack.Pop() * operandstack.Pop());
                                    break;
                                case "/":
                                    var divisor = operandstack.Pop();
                                    operandstack.Push(operandstack.Pop() / divisor);
                                    break;
                                case "(":
                                    keepLooping = false;
                                    break;
                            }
                            if (keepLooping)
                                operatorstack.Pop();
                        }
                        if (token == ")")
                            operatorstack.Pop();
                        else
                            operatorstack.Push(token);
                    }
                    else
                    {
                        if (Int32.TryParse(token, out int operand))
                            operandstack.Push(operand);
                        else
                            throw new ArgumentException();
                    };
                }

                if (operatorstack.Count > 0 || operandstack.Count > 1)
                    throw new ArgumentException();

                value = operandstack.Pop();
            }
            catch (Exception)
            {
                value = 0;
                eval_okay = false;
            }
            return eval_okay;
        }
    }
}
