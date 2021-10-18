using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Core
{
    public class CalculatorController : MonoBehaviour
    {
        [SerializeField] private CalculatorUiController _uiController;
        private readonly List<EquationElement> _equation = new List<EquationElement>();
        private CalculatorButtonType _currentType = CalculatorButtonType.None;

        private void Awake()
        {
            _uiController.SetEquationText(string.Empty);
            _uiController.SetResultText(string.Empty);
            _uiController.OnClickedButton += OnClickedButtonHandler;
            _uiController.OnSetFromHistory += OnSetFromHistoryHandler;
            _uiController.OnDeleteClick += OnDeleteClickHandler;
            _uiController.OnBoneClicked += OnBoneClickedHandler;
        }

        private void OnClickedButtonHandler(CalculatorButtonType type, string number)
        {
            switch (type)
            {
                case CalculatorButtonType.Cancel:
                    _currentType = type;
                    Cancel();
                    return;
                case CalculatorButtonType.Equal:
                    _currentType = type;
                    Equal();
                    return;
            }

            if (_currentType == CalculatorButtonType.Equal)
            {
                Cancel();
            }

            if (type != CalculatorButtonType.Number && type == _currentType)
                return;

            if (_currentType != CalculatorButtonType.None && !EquationElement.IsTypeForNumber(_currentType) &&
                _currentType != CalculatorButtonType.Brackets &&
                EquationElement.IsTypeArithmeticOperation(type))
                return;

            var lastItem = _equation.LastOrDefault();

            if (EquationElement.CheckListEmpty(type, number, lastItem, _equation))
            {
                _currentType = type;
                UpdateEquationText();
                return;
            }

            if (EquationElement.LastItemTypeWithSubList(type, number, lastItem, _equation))
            {
                _currentType = type;
                UpdateEquationText();
                return;
            }

            if (EquationElement.LastItemTypeNumber(type, number, lastItem, _equation))
            {
                _currentType = type;
                UpdateEquationText();
                return;
            }

            EquationElement item;
            if (EquationElement.IsTypeForNumber(type))
            {
                item = new EquationElement(CalculatorButtonType.Number);
                item.UpdateNumber(type, number);
            }
            else
            {
                item = new EquationElement(type);
            }

            _equation.Add(item);
            _currentType = type;

            if (EquationElement.IsTypeForNumber(type))
            {
                var output = PerformCalculation.Execute(_equation);
                _uiController.SetResultText(output.ToString(CultureInfo.CurrentCulture));
            }

            UpdateEquationText();
        }

        private void OnSetFromHistoryHandler(string text)
        {
            OnClickedButtonHandler(CalculatorButtonType.Number, text);
        }

        private void OnBoneClickedHandler(float value)
        {
            if (_currentType == CalculatorButtonType.Equal)
            {
                Cancel();
            }
            
            var lastItem = _equation.LastOrDefault();
            var type = CalculatorButtonType.Number;
            if (lastItem == null)
            {
                lastItem = new EquationElement(type);
                _equation.Add(lastItem);
            }
            else if (EquationElement.IsTypeWithSubList(lastItem.Type) && !lastItem.ClosedBracket)
            {
                var item = SearchLastElement(lastItem.SubList);
                if (item != null)
                {
                    lastItem = item;
                }
            }

            if (lastItem.Type == CalculatorButtonType.Number)
            {
                lastItem.Number.UpdateValue(value);
            }
            else
            {
                lastItem = new EquationElement(type);
                lastItem.Number.UpdateValue(value);
                _equation.Add(lastItem);
            }

            _currentType = type;
            UpdateEquationText();
        }

        private EquationElement SearchLastElement(List<EquationElement> list)
        {
            var lastItem = list.LastOrDefault();
            if (lastItem == null)
            {
                return null;
            }

            if (!EquationElement.IsTypeWithSubList(lastItem.Type) || lastItem.ClosedBracket) 
                return lastItem;
            
            var item = SearchLastElement(lastItem.SubList);
            if (item != null)
            {
                lastItem = item;
            }

            return lastItem;

        }

        private void Cancel()
        {
            _uiController.ResetBones();
            _equation.Clear();
            _currentType = CalculatorButtonType.None;
            _uiController.SetEquationText(string.Empty);
            _uiController.SetResultText(string.Empty);
        }

        private void Equal()
        {
            var output = PerformCalculation.Execute(_equation);
            var equationText = _equation.Aggregate(string.Empty, (current, element) => current + element) +
                               EquationElement.ToString(CalculatorButtonType.Equal);
            _uiController.AddHistoryItem(equationText, output.ToString(CultureInfo.CurrentCulture));
            _uiController.SetEquationText(equationText);
            _uiController.SetResultText(output.ToString(CultureInfo.CurrentCulture));
        }

        private void OnDeleteClickHandler()
        {
            _uiController.ResetBones();
            var lastItem = _equation.LastOrDefault();
            if (lastItem == null)
                return;

            if (_currentType == CalculatorButtonType.Equal)
                Cancel();

            if (EquationElement.IsTypeWithSubList(lastItem.Type))
            {
                var (count, buttonType) = lastItem.DeleteSubItem();
                if (count < 0)
                {
                    _equation.Remove(lastItem);
                    _currentType = _equation.Last()?.Type ?? CalculatorButtonType.None;
                }
                else
                {
                    _currentType = buttonType;
                }
            }
            else if (EquationElement.IsTypeForNumber(lastItem.Type))
            {
                var s = lastItem.Number.DeleteItem();
                if (string.IsNullOrEmpty(s))
                    _equation.Remove(lastItem);

                _currentType = _equation.Any() ? _equation.Last().Type : CalculatorButtonType.None;
            }
            else
            {
                _equation.Remove(lastItem);
                _currentType = _equation.Any() ? _equation.Last().Type : CalculatorButtonType.None;
            }

            UpdateEquationText();
            if (EquationElement.IsTypeForNumber(_currentType))
            {
                var output = PerformCalculation.Execute(_equation);
                _uiController.SetResultText(output.ToString(CultureInfo.CurrentCulture));
            }
            else
            {
                _uiController.SetResultText(string.Empty);
            }
        }

        private void UpdateEquationText()
        {
            _uiController.SetEquationText(_equation.Aggregate(string.Empty, (current, element) => current + element));
        }

        private void OnDestroy()
        {
            _uiController.OnClickedButton -= OnClickedButtonHandler;
            _uiController.OnSetFromHistory -= OnSetFromHistoryHandler;
            _uiController.OnDeleteClick -= OnDeleteClickHandler;
            _uiController.OnBoneClicked -= OnBoneClickedHandler;
        }
    }
}