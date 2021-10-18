using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class CalculatorTab: MonoBehaviour
    {
        public event Action<CalculatorTab> OnClicked;
        [SerializeField] private GameObject _tabContent;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            
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
            OnClicked?.Invoke(this);
        }

        public void ShowTabContent(bool show)
        {
            _tabContent.SetActive(show);
            gameObject.SetActive(!show);
        }
    }
}