using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Core
{
    public class AbacusBone : MonoBehaviour
    {
        public Action<int, int, int> OnBoneClicked;
        [Header("Image")] [SerializeField] private Sprite _bone_sprite;
        [SerializeField] private Sprite _bone_sprite_green;
        [Header("Settings")] [SerializeField] private Image _image;
        [SerializeField] private Button _button;

        private int _positionLine;
        private int _position;
        private int _positionMax;
        private float _positionXRight;
        private float _positionXLeft;
        public bool PositionRight { get; private set; }

        private void Awake()
        {
            _button.onClick.AddListener(OnBoneClickedHandler);
        }

        private void Start()
        {
            var transformPosition = transform.position;
            _positionXRight = UpdateX();
            _positionXLeft = UpdateXLeft();
            transform.position = new Vector3(_positionXRight, transformPosition.y, transformPosition.z);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnBoneClickedHandler);
        }

        private void OnBoneClickedHandler()
        {
            OnBoneClicked?.Invoke(_positionLine, _position, _positionMax);
        }

        public void Init(int positionLine, int position, int max)
        {
            PositionRight = true;
            _positionLine = positionLine;
            _position = position;
            _positionMax = max;

            if (position % 5 == 0 || position % 6 == 0)
            {
                _image.sprite = _bone_sprite_green;
            }
            else
            {
                _image.sprite = _bone_sprite;
            }
        }

        public void Move()
        {
            var position = transform.position;

            PositionRight = !PositionRight;
            var x = (PositionRight) ? _positionXRight : _positionXLeft;
            transform.DOMove(new Vector3(x, position.y, position.z), 1);
        }

        public void SetFirstPosition()
        {
            if(PositionRight)
                return;
            
            var position = transform.position;
            PositionRight = true;
            transform.position = new Vector3(_positionXRight, position.y, position.z);
        }

        private float UpdateX()
        {
            var p = _positionMax - _position;
            var recTransform = (RectTransform) transform;
            var x = Screen.width - (p * recTransform.rect.width * 1.3f);
            return x;
        }

        private float UpdateXLeft()
        {
            var p = _position;
            var recTransform = (RectTransform) transform;
            var x = (p * recTransform.rect.width * 1.3f) - (Screen.width/40f);
            return x;
        }
    }
}