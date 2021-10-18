using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class CalculatorUiController : MonoBehaviour
    {
        public event Action<CalculatorButtonType, string> OnClickedButton;
        public event Action<string> OnSetFromHistory;
        public event Action OnDeleteClick;
        public event Action<float> OnBoneClicked;
        
        [SerializeField] private CalculatorTab[] _tabs;
        [SerializeField] private CalculatorButton[] _buttons;
        [SerializeField] private TMP_Text _resultLabel;
        [SerializeField] private TMP_Text _equationLabel;
        [SerializeField] private Button _deleteButton;
        [Header("History tab")]
        [SerializeField] private Transform _content;
        [SerializeField] private HistoryItem _originLabel;
        private List<HistoryItem> _historyLabels = new List<HistoryItem>();
        [Header("Abacus")] 
        [SerializeField] private AbacusController _abacusController;

        public void SetEquationText(string text)
        {
            _equationLabel.text = text;
        }
        
        public void SetResultText(string text)
        {
            _resultLabel.text = text;
        }

        public void AddHistoryItem(string equation, string result)
        {
            var label = Instantiate(_originLabel, _content);
            label.InitItem(equation, result);
            label.gameObject.SetActive(true);
            label.OnSetFromHistory += OnSetFromHistoryHandler;
            
            _historyLabels.Add(label);
        }

        public void ResetBones()
        {
            _abacusController.ResetBones();
        }

        private void Awake()
        {
            bool first = true;
            foreach (var tab in _tabs)
            {
                tab.OnClicked += ChangeTab;
                tab.ShowTabContent(false);
                if (first)
                {
                    first = false;
                    tab.ShowTabContent(true);
                }
            }

            foreach (var btn in _buttons)
            {
                btn.OnClicked += ButtonClickHandler;
            }
            _deleteButton.onClick.AddListener(OnDeleteClickHandler);
            _abacusController.OnBoneClicked += OnBoneClickedHandler;
        }

        private void ChangeTab(CalculatorTab calculatorTab)
        {
            foreach (var tab in _tabs)
            {
                tab.ShowTabContent(false);
            }
            
            calculatorTab.ShowTabContent(true);
        }

        private void ButtonClickHandler(CalculatorButtonType type, string number)
        {
            OnClickedButton?.Invoke(type, number);
        }

        private void OnSetFromHistoryHandler(string result)
        {
            OnSetFromHistory?.Invoke(result);
        }

        private void OnDeleteClickHandler()
        {
            OnDeleteClick?.Invoke();
        }

        private void OnBoneClickedHandler(float value)
        {
            OnBoneClicked?.Invoke(value);
        }

        private void OnDestroy()
        {
            _deleteButton.onClick.RemoveListener(OnDeleteClickHandler);
            _abacusController.OnBoneClicked -= OnBoneClickedHandler;
            Array.ForEach(_tabs, tab =>  tab.OnClicked -= ChangeTab);
            Array.ForEach(_buttons, btn =>  btn.OnClicked -= ButtonClickHandler);
            _historyLabels.ForEach(lbl =>  lbl.OnSetFromHistory -= OnSetFromHistoryHandler);
        }
    }
}
