using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Core
{
    [ExecuteInEditMode]
    public class CalculatorButtonSettings: MonoBehaviour
    {
        [SerializeField] private TMP_Text _textLabel;
        [SerializeField] private Image _iconLabel;

        [Header("Settings")] 
        public string Text;
        public Color TextColor;
        public Sprite SpriteIcon;

        private void Update()
       {
           if(!_textLabel || !_iconLabel) return;
           
           _textLabel.gameObject.SetActive(false);
           _iconLabel.gameObject.SetActive(false);

           if (!Text.Equals(string.Empty))
           {
               _textLabel.text = Text;
               _textLabel.color = TextColor;
               _textLabel.gameObject.SetActive(true);
               return;
           }

           if (!SpriteIcon.Equals(null))
           {
               _iconLabel.sprite = SpriteIcon;
               _iconLabel.gameObject.SetActive(true);
           }
       }
    }
}