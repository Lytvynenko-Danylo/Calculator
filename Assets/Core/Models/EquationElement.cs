using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class EquationElement
    {
        #region static members

        /// <summary>
        /// Список строковых представлений CalculatorButtonType
        /// </summary>
        private static readonly Dictionary<CalculatorButtonType, string> STRING_TYPE =
            new Dictionary<CalculatorButtonType, string>()
            {
                {CalculatorButtonType.Difference, "-"},
                {CalculatorButtonType.Divide, "/"},
                {CalculatorButtonType.Increase, "*"},
                {CalculatorButtonType.Equal, "="},
                {CalculatorButtonType.Percent, "%"},
                {CalculatorButtonType.Summation, "+"},
            };

        /// <summary>
        /// Если тип имеет под список, н-р скобки, синус
        /// </summary>
        public static bool IsTypeWithSubList(CalculatorButtonType type)
        {
            switch (type)
            {
                case CalculatorButtonType.Brackets:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Если тип число или связан с его преобразованием
        /// </summary>
        public static bool IsTypeForNumber(CalculatorButtonType type)
        {
            switch (type)
            {
                case CalculatorButtonType.Number:
                case CalculatorButtonType.DecimalSeparator:
                case CalculatorButtonType.NumberSign:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Если тип арифметическое действие
        /// </summary>
        public static bool IsTypeArithmeticOperation(CalculatorButtonType type)
        {
            switch (type)
            {
                case CalculatorButtonType.Difference:
                case CalculatorButtonType.Divide:
                case CalculatorButtonType.Percent:
                case CalculatorButtonType.Increase:
                case CalculatorButtonType.Summation:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Если список пустой добавить элемент список
        /// </summary>
        public static bool CheckListEmpty(CalculatorButtonType type, string number, EquationElement lastItem,
            List<EquationElement> list)
        {
            if (lastItem == null)
            {
                if (IsTypeForNumber(type))
                {
                    lastItem = new EquationElement(CalculatorButtonType.Number);
                    lastItem.UpdateNumber(type, number);
                }
                else
                {
                    lastItem = new EquationElement(type);
                }

                list.Add(lastItem);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверяем, что последний элемент имеет под список и переходим в него
        /// </summary>
        public static bool LastItemTypeWithSubList(CalculatorButtonType type, string number, EquationElement lastItem,
            List<EquationElement> list)
        {
            if (IsTypeWithSubList(lastItem.Type))
            {
                //последний эллемент закрытая скобка
                if (lastItem.ClosedBracket)
                {
                    EquationElement item;
                    if (IsTypeForNumber(type))
                    {
                        item = new EquationElement(CalculatorButtonType.Number);
                        item.UpdateNumber(type, number);
                    }
                    else
                    {
                        item = new EquationElement(type);
                    }

                    list.Add(item);
                }
                else
                {
                    lastItem.UpdateSublist(type, number);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверяем, что последний элемент число
        /// </summary>
        public static bool LastItemTypeNumber(CalculatorButtonType type, string number, EquationElement lastItem,
            List<EquationElement> list)
        {
            if (lastItem.Type == CalculatorButtonType.Number)
            {
                if (IsTypeForNumber(type))
                {
                    lastItem.UpdateNumber(type, number);
                }
                else
                {
                    list.Add(new EquationElement(type));
                }

                return true;
            }

            return false;
        }

        #endregion

        public CalculatorButtonType Type { get; }
        public bool ClosedBracket { get; private set; }
        public EquationNumber Number { get; }
        public List<EquationElement> SubList { get; }

        public EquationElement(CalculatorButtonType type)
        {
            Type = type;
            switch (type)
            {
                case CalculatorButtonType.Brackets:
                    SubList = new List<EquationElement>();
                    break;
                case CalculatorButtonType.Number:
                    Number = new EquationNumber();
                    break;
            }
        }

        public void UpdateNumber(CalculatorButtonType type, string number)
        {
            Number.Update(type, number);
        }

        public void UpdateSublist(CalculatorButtonType type, string number)
        {
            var lastItem = SubList.LastOrDefault();
            //закрывающая скобка
            if (type == CalculatorButtonType.Brackets)
            {
                if (lastItem == null || lastItem.Type == CalculatorButtonType.Number)
                {
                    ClosedBracket = true;
                    return;
                }
            }

            if (CheckListEmpty(type, number, lastItem, SubList))
                return;

            if (LastItemTypeWithSubList(type, number, lastItem, SubList))
                return;

            if (LastItemTypeNumber(type, number, lastItem, SubList))
                return;

            EquationElement item;
            if (IsTypeForNumber(type))
            {
                item = new EquationElement(CalculatorButtonType.Number);
                item.UpdateNumber(type, number);
            }
            else
            {
                item = new EquationElement(type);
            }

            SubList.Add(item);
        }

        public (int Count, CalculatorButtonType Type) DeleteSubItem()
        {
            var lastItem = SubList.LastOrDefault();
            if (lastItem == null)
                return (-1, Type);
            
            CalculatorButtonType type;

            if (IsTypeWithSubList(lastItem.Type))
            {
                var (count, calculatorButtonType) = lastItem.DeleteSubItem();
                type = calculatorButtonType;
                if (count >= 0) return 
                    (SubList.Count, type);
                
                SubList.Remove(lastItem);
                type = SubList.Any() ? SubList.Last().Type : Type;

            }
            else if (ClosedBracket)
            {
                ClosedBracket = false;
                type = lastItem.Type;
            }
            else if (IsTypeForNumber(lastItem.Type))
            {
                var s = lastItem.Number.DeleteItem();
                if (string.IsNullOrEmpty(s))
                    SubList.Remove(lastItem);

                type = SubList.Any() ? SubList.Last().Type : Type;
            }
            else
            {
                SubList.Remove(lastItem);
                type = SubList.Any() ? SubList.Last().Type : Type;
            }

            return (SubList.Count, type);
        }

        public override string ToString()
        {
            switch (Type)
            {
                case CalculatorButtonType.Brackets:
                    var str = SubList.Aggregate(" (", (current, item) => current + item);
                    return ClosedBracket ? str + ")" : str;
                case CalculatorButtonType.Number:
                    return " " + Number;
                default:
                    return " " + STRING_TYPE[Type];
            }
        }

        public static string ToString(CalculatorButtonType type)
        {
            return " " + STRING_TYPE[type];
        }
    }
}