using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerraCombatTesting.Logic
{
    class InfixToPostFix
    {
        // https://stackoverflow.com/questions/13421424/how-to-evaluate-an-infix-expression-in-just-one-scan-using-stacks
        // https://stackoverflow.com/questions/44188609/how-to-calculate-output-of-infix-expression-by-using-stacks-in-c-sharp 

        public static int ComputeInfixOrginal(string infix)
        {
            var operatorstack = new Stack<char>();
            var operandstack = new Stack<int>();

            var precedence = new Dictionary<char, int> { { '(', 0 }, { '*', 1 }, { '/', 1 }, { '+', 2 }, { '-', 2 }, { ')', 3 } };

            foreach (var ch in $"({infix})")
            {
                switch (ch)
                {
                    case var digit when Char.IsDigit(digit):
                        operandstack.Push(Convert.ToInt32(digit.ToString()));
                        break;
                    case var op when precedence.ContainsKey(op):
                        var keepLooping = true;
                        while (keepLooping && operatorstack.Count > 0 && precedence[ch] > precedence[operatorstack.Peek()])
                        {
                            switch (operatorstack.Peek())
                            {
                                case '+':
                                    operandstack.Push(operandstack.Pop() + operandstack.Pop());
                                    break;
                                case '-':
                                    operandstack.Push(-operandstack.Pop() + operandstack.Pop());
                                    break;
                                case '*':
                                    operandstack.Push(operandstack.Pop() * operandstack.Pop());
                                    break;
                                case '/':
                                    var divisor = operandstack.Pop();
                                    operandstack.Push(operandstack.Pop() / divisor);
                                    break;
                                case '(':
                                    keepLooping = false;
                                    break;
                            }
                            if (keepLooping)
                                operatorstack.Pop();
                        }
                        if (ch == ')')
                            operatorstack.Pop();
                        else
                            operatorstack.Push(ch);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            if (operatorstack.Count > 0 || operandstack.Count > 1)
                throw new ArgumentException();

            return operandstack.Pop();
        }

        private static readonly Dictionary<string, int> _precedence = new Dictionary<string, int> { { "(", 0 }, { "*", 1 }, { "/", 1 }, { "+", 2 }, { "-", 2 }, { ")", 3 } };

        public static int ComputeInfix(List<string> infix)
        {
            var operatorstack = new Stack<string>();
            var operandstack = new Stack<int>();

            // Wrap with parenthisis 
            infix.Insert(0, "(");
            infix.Add(")");

            foreach (var token in infix)
            {
                if (_precedence.ContainsKey(token)) // If token in an operator
                {
                    var keepLooping = true;
                    while (keepLooping && operatorstack.Count > 0 && _precedence[token] > _precedence[operatorstack.Peek()])
                    {
                        switch (operatorstack.Peek())
                        {
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
                    if (Int32.TryParse (token, out int value))
                        operandstack.Push(value);
                    else
                        throw new ArgumentException();
                };
            }

            if (operatorstack.Count > 0 || operandstack.Count > 1)
                throw new ArgumentException();

            return operandstack.Pop();
        }
    }
}


