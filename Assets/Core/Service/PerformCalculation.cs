using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Core
{
    public static class PerformCalculation
    {
        public static float Execute(List<EquationElement> equation)
        {
            if (equation == null || !equation.Any())
            {
                return 0;
            }

            List<EquationElement> list = new List<EquationElement>();
            var lastUsedNumber = -1;
            for (var i = 0; i < equation.Count; i++)
            {
                switch (equation[i].Type)
                {
                    case CalculatorButtonType.Divide:
                    case CalculatorButtonType.Increase:
                    case CalculatorButtonType.Percent:
                    {
                        var last = list.Last();
                        var number = ArithmeticOperation(equation[i].Type, last,
                            i == equation.Count - 1 ? last : equation[i + 1]);
                        last = new EquationElement(CalculatorButtonType.Number);
                        last.UpdateNumber(CalculatorButtonType.Number, number.ToString(CultureInfo.CurrentCulture));
                        list[list.Count - 1] = last;
                        lastUsedNumber = i + 1;
                    }
                        break;
                    default:
                        if (i != lastUsedNumber)
                        {
                            list.Add(equation[i]);
                        }

                        break;
                }
            }

            float sum = 0;
            for (var i = 0; i < list.Count; i++)
            {
                switch (list[i].Type)
                {
                    case CalculatorButtonType.Difference:
                    case CalculatorButtonType.Summation:
                    {
                        if (i == list.Count - 1)
                        {
                            sum = ArithmeticOperation(list[i].Type, list[i-1], list[i-1]);
                            var item = new EquationElement(CalculatorButtonType.Number);
                            item.UpdateNumber(CalculatorButtonType.Number, sum.ToString(CultureInfo.CurrentCulture));
                            list[i] = item;
                        }
                        else
                        {
                            sum = ArithmeticOperation(list[i].Type, list[i-1], list[i + 1]);
                            var item = new EquationElement(CalculatorButtonType.Number);
                            item.UpdateNumber(CalculatorButtonType.Number, sum.ToString(CultureInfo.CurrentCulture));
                            list[i + 1] = item;
                        }
                    }
                        break;
                }
            }
            
            sum = list.Last().Number.Value;
            return sum;
        }

        private static float ArithmeticOperation(CalculatorButtonType type, EquationElement arg1, EquationElement arg2)
        {
            var value1 = GetValue(arg1);

            var value2 = GetValue(arg2);

            switch (type)
            {
                case CalculatorButtonType.Divide:
                    return value1 / value2;
                case CalculatorButtonType.Increase:
                    return value1 * value2;
                case CalculatorButtonType.Percent:
                    return value1 % value2;
                //
                case CalculatorButtonType.Difference:
                    return value1 - value2;
                case CalculatorButtonType.Summation:
                    return value1 + value2;
            }

            return 0;
        }

        private static float GetValue(EquationElement arg)
        {
            if (arg.SubList != null && arg.SubList.Any())
            {
                return Execute(arg.SubList);
            }

            return arg.Number.Value;
        }

    }
}