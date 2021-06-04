using Spreadsheet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spreadsheet.Models
{
  
    public class SheetValueInterpreter
    {
        private readonly ApplicationDbContext applicationDbContext;
       
       

        public SheetValueInterpreter(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }
        public string Interprete(string input, int sheedID)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            string errorMessage = "";
            string inputBeforeChanges = input;
            int divider = 1;
            //a1+11-8*(42+2)
            try
            {
                input = input.Trim();
                input = input.Replace(" ", string.Empty); // to allow user to write a1 + a1 insted of a1+a1

                if (input.StartsWith("avg") || input.StartsWith("AVG"))
                {
                    input = AverageFunction(input, ref divider);
                }

             
                LinkedList<string> operandsAndOperators = new LinkedList<string>();
                var inputCharacters = input.ToCharArray();
                int indexOfInput = 0;
                bool multipleByMinusOne = false;

                while (indexOfInput < inputCharacters.Length)
                {
                    var currentCharacter = inputCharacters[indexOfInput];
                    if (currentCharacter == '-')
                    {
                        if (operandsAndOperators.Count == 0
                            || operandsAndOperators.Last.Value == "+" || operandsAndOperators.Last.Value == "-"
                            || operandsAndOperators.Last.Value == "*" || operandsAndOperators.Last.Value == "/ "
                            || operandsAndOperators.Last.Value == "(") //operandsAndOperators.Last.Value.IndexOfAny(new char[] { '+', '-', '*', '/','(' }) != 0
                        {
                            multipleByMinusOne = true;
                            indexOfInput++;
                            continue;
                        }


                    }
                    // call for field in sheet
                    if (char.IsLetter(currentCharacter))
                    {
                        currentCharacter = char.ToUpper(currentCharacter);
                        int x = Convert.ToInt32(currentCharacter) - 65 + 1; // numbering from 1... 

                        var digits = input.Substring(++indexOfInput).TakeWhile(c => char.IsDigit(c));
                        StringBuilder digitsStringBuilder = new StringBuilder();
                        foreach (var item in digits)
                        {
                            digitsStringBuilder.Append(item);
                            indexOfInput++;
                        }
                        var y = int.Parse(digitsStringBuilder.ToString());
                        SheetValue sheetValue = applicationDbContext.sheetValues.Where(v => v.SheetID == sheedID && v.X == x && v.Y == y).FirstOrDefault();

                        if (sheetValue != null)
                        {
                            if (double.TryParse(sheetValue.Value, out double numberValue))
                            {
                                numberValue = multipleByMinusOne ? numberValue * (-1) : numberValue;
                                multipleByMinusOne = false;
                                operandsAndOperators.AddLast(numberValue.ToString());
                            }
                            else
                            {
                                errorMessage = $"field  {currentCharacter}{y} is not a number";
                                throw new Exception();
                            }


                        }
                        else
                        {
                            errorMessage = $"there is no field  {currentCharacter}{y} or its not set";
                            throw new Exception();
                        }



                    }
                    // poczatek liczby moze byc cyfra
                    else if (char.IsDigit(currentCharacter))
                    {
                        var digits = input.Substring(indexOfInput).TakeWhile(c => char.IsDigit(c) || c == ',');
                        StringBuilder digitsStringBuilder = new StringBuilder();
                        foreach (var item in digits)
                        {
                            digitsStringBuilder.Append(item);
                            indexOfInput++;
                        }
                        double numberValue = double.Parse(digitsStringBuilder.ToString());
                        numberValue = multipleByMinusOne ? numberValue * (-1) : numberValue;
                        multipleByMinusOne = false;
                        operandsAndOperators.AddLast(numberValue.ToString());
                    }
                    // must be operator or bracket
                    else
                    {
                        operandsAndOperators.AddLast(inputCharacters[indexOfInput].ToString());
                        indexOfInput++;
                    }
                }
                var RPN = GetReversePolishNotation(operandsAndOperators.ToArray());
                var valueRPN = RPN_ToValue(RPN) / divider;
                return string.Format("{0:0.##}", valueRPN);

            }
            catch (Exception e)
            {
                errorMessage = errorMessage == "" ? inputBeforeChanges : "[ " + errorMessage + " ]" + inputBeforeChanges;
                return errorMessage;
            }

        }

        public string AverageFunction(string input, ref int divider)
        {
            divider = input.Where(c => c == ',').Count() + 1;
            var a = input.Last();
            if (input[3] == '(' && input.Last() == ')')
            return input = string.Join("+", input.Substring(4, input.Length - 5).Split(","));

            return input;
        }

        public string[] GetReversePolishNotation(string[] operandsAndOperatos)
        {
            Stack<string> stack = new Stack<string>();
            List<string> listToReversePolishNotation = new List<string>();
            Dictionary<string, int> operandsImportance = new Dictionary<string, int>(){
            {"(",0 },
           {"-",1},  { "+", 1 },
           {"/",2} ,{"*",2},
        };

            foreach (var item in operandsAndOperatos)
            {
                if (double.TryParse(item, out double number))
                {
                    listToReversePolishNotation.Add(number.ToString());
                }
                else
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(item);
                        continue;
                    }

                    if (item == "(")
                    {
                        stack.Push(item);
                        continue;
                    }

                    if (item == ")")
                    {
                        while (true)
                        {
                            string itemPoped = stack.Peek();
                            if (itemPoped == "(")
                            {
                                stack.Pop();
                                break;
                            }
                            else
                            {
                                listToReversePolishNotation.Add(itemPoped);
                                stack.Pop();
                            }
                        }
                        continue;
                    }
                    string operatorPeek = stack.Peek();
                    if (operandsImportance[item] > operandsImportance[operatorPeek])
                    {
                        stack.Push(item);
                        continue;
                    }
                    else
                    {

                        while (stack.Count > 0 && operandsImportance[item] <= operandsImportance[stack.Peek()])
                        {
                            listToReversePolishNotation.Add(stack.Pop());
                        }

                        stack.Push(item);
                    }



                }
            }
            while (stack.Count > 0)
            {
                listToReversePolishNotation.Add(stack.Pop());
            }
            return listToReversePolishNotation.ToArray<string>();

        }
        public double RPN_ToValue(string[] RPN_Array)
        {

            Stack<double> stackValues = new Stack<double>();

            foreach (var item in RPN_Array)
            {
                if (double.TryParse(item, out double number))
                {
                    stackValues.Push(Convert.ToDouble(item.ToString()));
                }
                else
                {

                    double firstOperand = stackValues.Pop();
                    double SecondOperand = stackValues.Pop();
                    double value;
                    switch (item)
                    {
                        case "+":
                            value = firstOperand + SecondOperand;
                            stackValues.Push(value);
                            break;
                        case "-":
                            value = SecondOperand - firstOperand;
                            stackValues.Push(value);
                            break;
                        case "*":
                            value = firstOperand * SecondOperand;
                            stackValues.Push(value);
                            break;

                        case "/":
                            value = SecondOperand / firstOperand;
                            stackValues.Push(value);
                            break;
                        default:
                            break;
                    }

                }
            }

            return stackValues.Pop();
        }
    }
}
