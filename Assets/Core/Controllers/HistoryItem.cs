using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class HistoryItem: MonoBehaviour
    {
        public event Action<string> OnSetFromHistory; 
        [SerializeField]private Button _button;
        [SerializeField] private TMP_Text _resultLabel;
        [SerializeField] private TMP_Text _equationLabel;

        private void Awake()
        {
            _button.onClick.AddListener(OnClickHandler);
        }

        public void InitItem(string equation, string result)
        {
            _resultLabel.text = result;
            _equationLabel.text = equation;
        }

        private void OnClickHandler()
        {
            OnSetFromHistory?.Invoke(_resultLabel.text);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClickHandler);
        }
    }
}