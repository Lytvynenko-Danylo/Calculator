using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    public class AbacusController : MonoBehaviour
    {
        private readonly int LINE_SEPARATOR = 3;
        public event Action<float> OnBoneClicked;
        [SerializeField] private GameObject _lineOrigin;
        [SerializeField] private AbacusBone _boneOrigin;

        List<List<AbacusBone>> _bones;

        public void ResetBones()
        {
            if (_bones == null || !_bones.Any()) return;
            foreach (var bone in _bones.SelectMany(line => line))
            {
                bone.SetFirstPosition();
            }
        }

        private void Start()
        {
            _bones = new List<List<AbacusBone>>();
            for (var i = 0; i < 12; i++)
            {
                var list = new List<AbacusBone>();
                var line = Instantiate(_lineOrigin, transform);
                line.SetActive(true);
                var maxBones = (i == LINE_SEPARATOR) ? 5 : 11;
                for (var y = 1; y < maxBones; y++)
                {
                    var bone = Instantiate(_boneOrigin, line.transform);
                    bone.gameObject.SetActive(true);
                    bone.Init(i, y, maxBones);
                    bone.OnBoneClicked += OnBoneClickedHandler;
                    list.Add(bone);
                }

                _bones.Add(list);
            }
        }

        private void OnEnable()
        {
            ResetBones();
        }

        private void OnBoneClickedHandler(int linePosition, int position, int max)
        {
            var step = 0f;
            if (linePosition > LINE_SEPARATOR)
            {
                step = (float) Math.Pow(10, linePosition - 4);
            }
            else if (linePosition < LINE_SEPARATOR)
            {
                step = (float) Math.Pow(10, linePosition - 3);
            }

            var res = 0f;
            if (_bones[linePosition][position - 1].PositionRight)
            {
                for (var i = 0; i < position; i++)
                {
                    if (_bones[linePosition][i].PositionRight == _bones[linePosition][position - 1].PositionRight)
                    {
                        res += step;
                        _bones[linePosition][i].Move();
                    }
                }
            }
            else
            {
                for (var i = max - 2; i > position - 2; i--)
                {
                    if (_bones[linePosition][i].PositionRight == _bones[linePosition][position - 1].PositionRight)
                    {
                        res -= step;
                        _bones[linePosition][i].Move();
                    }
                }
            }

            if (linePosition == LINE_SEPARATOR)
                return;
            
            OnBoneClicked?.Invoke(res);
        }

        private void OnDestroy()
        {
            foreach (var bone in _bones.SelectMany(line => line))
            {
                bone.OnBoneClicked -= OnBoneClickedHandler;
            }
        }
    }
}