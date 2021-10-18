using System;
using System.Globalization;

namespace Core
{
    public class EquationNumber
    {
        private static readonly char DECIMALSEPARATOR = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        private static readonly string ZERO = "0";
        
        private string _text;
        
        public float Value { get;  private set; }
        public bool IsFloat { get;  private set; }
        public bool IsNegative { get; private set; }
        
        
        private float ParseValue()
        {
            if (!float.TryParse(_text, out var result))
            {
                result = default;
            }

            if (IsNegative)
                result *= -1;

            return result;
        }

        public void UpdateValue(float value)
        {
            Value += value;
            IsNegative = (Value < 0);
            value = Math.Abs(Value);
            IsFloat = ((value - Math.Truncate(value)) > 0);
            _text = value.ToString(CultureInfo.CurrentCulture);
        }

        public string DeleteItem()
        {
            int lastPosition = _text.Length - 1;
            if (_text[lastPosition] == DECIMALSEPARATOR)
            {
                IsFloat = false;
            }
            _text = _text.Remove(lastPosition);
            Value = ParseValue();
            return _text;
        }

        public void Update(CalculatorButtonType type, string number)
        {
            switch (type)
            {
                case CalculatorButtonType.NumberSign:
                    IsNegative = !IsNegative;
                    if (string.IsNullOrEmpty(_text))
                    {
                        _text = ZERO;
                    }

                    Value = ParseValue();
                    return;
                case CalculatorButtonType.DecimalSeparator when IsFloat:
                    return;
                case CalculatorButtonType.DecimalSeparator:
                {
                    IsFloat = true;
                    if (string.IsNullOrEmpty(_text))
                    {
                        _text = ZERO;
                    }

                    _text += DECIMALSEPARATOR;
                    break;
                }
                case CalculatorButtonType.Number when string.IsNullOrEmpty(_text):
                case CalculatorButtonType.Number when _text.Equals(ZERO):
                    _text = number;
                    break;
                case CalculatorButtonType.Number:
                    _text += number;
                    break;
            }
            Value = ParseValue();
        }
        
        

        public override string ToString()
        {
            return IsNegative ? "-"+_text : _text;
        }
    }
}