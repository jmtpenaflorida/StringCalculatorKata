using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace StringCalculatorKata
{
    public class StringCalculatorTests
    {
        [Fact]
        public void InputEmptyStringThenCallAddExpectZero()
        {
            var calculator = new StringCalculator();

            Assert.Equal(0, calculator.Add(String.Empty));
            Assert.Equal(0, calculator.Add(" "));
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("1,2", 3)]
        [InlineData("1,2,3", 6)]
        [InlineData("1001", 0)]
        [InlineData("2,1001", 2)]
        public void InputStringsWithCommaDelimiterThenCallAddExpectSum(string numbers, int result)
        {
            var calculator = new StringCalculator();

            Assert.Equal(result, calculator.Add(numbers));
        }

        [Theory]
        [InlineData("1\n2,3", 6)]
        [InlineData("//;\n1;2;3", 6)]
        public void InputStringsWithNewLineDelimeterThenCallAddExpectSum(string numbers, int result)
        {
            var calculator = new StringCalculator();

            Assert.Equal(result, calculator.Add(numbers));
        }

        [Fact]
        public void InputNegativeStringThenCallAddThrowAnException()
        {
            var calculator = new StringCalculator();

            Assert.Equal("Negatives not allowed: -1", Assert.Throws<ArgumentException>(() => calculator.Add("-1")).Message);
            Assert.Equal("Negatives not allowed: -3,-1,-2", Assert.Throws<ArgumentException>(() => calculator.Add("-3,1,-1,-2")).Message);
        }
    }

    public class StringCalculator
    {
        private const string DEFAULT_DELIMITER = "[,\\n]";
        private const string USER_DEFINED_DELIMETER = "^//\\D\\n";

        public int Add(string numbers)
        {
            numbers = numbers.Trim();

            if (numbers.Length == 0)
                return 0;

            var delimiter = SetupVariables(ref numbers);

            var numbersList = GetNumbersByDelimeter(numbers, delimiter);

            var negativeNumbersList = numbersList.Where(x => int.Parse(x) < 0);

            if (negativeNumbersList.Count() > 0)
                throw new ArgumentException("Negatives not allowed: " + String.Join(",", negativeNumbersList));

            return Sum(numbersList);
        }

        private static int Sum(string[] numbersList)
        {
            int sum = 0;

            foreach (var numberString in numbersList)
            {
                int number = int.Parse(numberString);

                if (number > 1000) number = 0;

                sum += number;
            }

            return sum;
        }

        private string[] GetNumbersByDelimeter(string numbers, string delimeter)
        {
            return new Regex(delimeter).Replace(numbers, " ").Split(" ");
        }

        private string SetupVariables(ref string numbers)
        {
            var userDefinedDelimeterRegex = new Regex(USER_DEFINED_DELIMETER);
            var userDefinedDelimeter = userDefinedDelimeterRegex.Match(numbers).Value;

            var delimiter = DEFAULT_DELIMITER;

            if (userDefinedDelimeterRegex.Match(numbers).Success)
            {
                delimiter = numbers.Substring(0, userDefinedDelimeter.Length).Substring(2).Substring(0, 1);

                numbers = numbers.Substring(userDefinedDelimeter.Length);
            }

            return delimiter;
        }

       
    }
}
