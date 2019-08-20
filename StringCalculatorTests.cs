using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace StringCalculatorKata
{
    public class StringCalculatorTests
    {
        private StringCalculator _calculator;
        
        public StringCalculatorTests()
        {
            _calculator = new StringCalculator();
        }

        [Fact]
        public void InputEmptyStringThenCallAddExpectZero()
        {
            Assert.Equal(0, _calculator.Add(String.Empty));
            Assert.Equal(0, _calculator.Add(" "));
            Assert.Equal(0, _calculator.Add(""));
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("1,2", 3)]
        [InlineData("1,2,3", 6)]
        [InlineData("1001", 0)]
        [InlineData("2,1001", 2)]
        public void InputStringsWithCommaDelimiterThenCallAddExpectSum(string numbers, int result)
        {
            Assert.Equal(result, _calculator.Add(numbers));
        }

        [Theory]
        [InlineData("1\n2,3", 6)]
        [InlineData("//;\n1;2;3", 6)]
        public void InputStringsWithDifferentDelimeterThenCallAddExpectSum(string numbers, int result)
        {
            Assert.Equal(result, _calculator.Add(numbers));
        }

        [Theory]
        [InlineData("-1", "-1")]
        [InlineData("-3,1,-1,-2", "-3,-1,-2")]
        [InlineData("//[***]\n1***-2***3", "-2")]
        [InlineData("3,1\n-1,-2", "-1,-2")]
        public void InputNegativeStringThenCallAddThrowAnException(string numbers, string result)
        {
            Assert.Equal("Negatives not allowed: " + result, Assert.Throws<ArgumentException>(() => _calculator.Add(numbers)).Message);                        
        }

        [Theory]       
        [InlineData("//[***]\n1***2***3", 6)]
        [InlineData("//[@@@@]\n1@@@@2000@@@@3", 4)]
        [InlineData("//[*][%]\n1*2%3", 6)]
        [InlineData("//[*][%][##]\n1*2%3##4", 10)]
        [InlineData("//[*][%][##][}]\n1*2%3##4}5", 15)]
        [InlineData("//[--]\n1--2000--3", 4)]
        public void InputStringsWithRangedDelimeterThenCallAddExpectSum(string numbers, int result)
        {
            Assert.Equal(result, _calculator.Add(numbers));
        }       
    }

    public class StringCalculator
    {
        private const string DEFAULT_DELIMITER = "[,\\n]";
        private const string USER_DEFINED_DELIMETER = "^//.*\\n";

        public int Add(string numbers)
        {
            numbers = numbers.Trim();

            if (numbers.Length == 0) return 0;

            var delimiterVariables = SetupDelimiterVariables(numbers);

            var numbersList = GetNumbersByDelimiter(delimiterVariables.numbers, delimiterVariables.delimiter).Where(x => x.Length != 0);
            
            var negativeNumbersList = numbersList.Where(x => int.Parse(x) < 0);

            if (negativeNumbersList.Count() > 0) throw new ArgumentException("Negatives not allowed: " + String.Join(",", negativeNumbersList));

            return Sum(numbersList);
        }

        private static int Sum(IEnumerable<string> numbersList)
        {
            return numbersList.Select(x => {
                int number = int.Parse(x); 

                if(number > 1000) return 0;
                
                return number;
            }).Sum();           
        }

        private string[] GetNumbersByDelimiter(string numbers, string delimiter)
        {
            return new Regex(delimiter).Replace(numbers, " ").Split(" ");
        }

        private (string numbers, string delimiter) SetupDelimiterVariables(string numbers)
        {
            var userDefinedDelimiterRegex = new Regex(USER_DEFINED_DELIMETER);
            var userDefinedDelimiter = userDefinedDelimiterRegex.Match(numbers).Value;

            var delimiter = DEFAULT_DELIMITER;

            if (userDefinedDelimiterRegex.Match(numbers).Success)
            {
                delimiter = numbers.Substring(0, userDefinedDelimiter.Length).Substring(2);

                if (userDefinedDelimiter.Contains("[") && userDefinedDelimiter.Contains("]"))
                {
                    delimiter = delimiter.Substring(0, delimiter.IndexOf("\n"));
                    delimiter = String.Format("[{0}]", delimiter.Replace("[", String.Empty).Replace("]", String.Empty));
                }
                else
                    delimiter = delimiter.Substring(0, 1);

                numbers = numbers.Substring(userDefinedDelimiter.Length);
            }

            return (numbers, delimiter);
        }
    }
}
