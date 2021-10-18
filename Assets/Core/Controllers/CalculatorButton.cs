using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    [ExecuteInEditMode]
    public class CalculatorButton : MonoBehaviour
    {
        public event Action<CalculatorButtonType, string> OnClicked;
        
        [SerializeField] private Button _button = default;
        [SerializeField] private CalculatorButtonSettings _settings = default;
        [Header("Button Type")]
        public CalculatorButtonType Type = default;

        private void Awake()
        {

            if (_button)
            {
                _button.onClick.AddListener(OnClickedHandler);
            }
        }

        private void OnDestroy()
        {
            if (_button)
            {
                _button.onClick.RemoveListener(OnClickedHandler);
            }
        }

        private void OnClickedHandler()
        {
            OnClicked?.Invoke(Type, _settings.Text);
        }
    }
}
