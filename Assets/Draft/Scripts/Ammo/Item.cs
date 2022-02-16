using System;
using Draft.Controller;
using Modules.Utilities;
using UniRx;
using UnityEngine;

namespace Draft.Ammo
{
    public class Item : MonoBehaviour, IPoolObjectEvent
    {
        public enum ItemType
        {
            Missile,
            Shield,
            Life
        }

        private Transform _Transform;

        [SerializeField] private ItemType m_ItemType = ItemType.Missile;
        [SerializeField] public int m_ScoreAdd = 100;

        void Start()
        {
        }

        private Vector3 ClampAreaScreen(Vector3 _input)
        {
            var areaHeight = 2f * Camera.main.orthographicSize;
            var areaWidth = areaHeight * Camera.main.aspect;
            var topPos = (areaHeight / 2f) - 0.5f;
            var bottomPos = -(areaHeight / 2f) + 0.5f;
            var leftPos = -(areaWidth / 2f) + 0.5f;
            var rightPos = (areaWidth / 2f) - 0.5f;

            _input.x = Mathf.Clamp(_input.x, leftPos, rightPos);
            _input.y = Mathf.Clamp(_input.y, bottomPos, topPos);
            return _input;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {

                if (m_ItemType == ItemType.Missile)
                {
                    AudioManager.PlayFX("missile.item");
                    PlayerController.Instance.EnableMissileProjectile();
                }
                else if (m_ItemType == ItemType.Shield)
                {
                    AudioManager.PlayFX("life.item");
                    PlayerController.Instance.EnableShield();
                }
                else if (m_ItemType == ItemType.Life)
                {
                    AudioManager.PlayFX("life.item");
                    PlayerController.Instance.AddLifeValue(1);
                }

                PlayerController.Instance.AddScoreValue(m_ScoreAdd);
                ObjectPoolingManager.Kill(gameObject);
            }
        }

        private IDisposable UpdateDisposable;

        public void OnStartObject()
        {
            _Transform = transform;

            _Transform.position = ClampAreaScreen(transform.position);

            var areaHeight = 2f * Camera.main.orthographicSize;
            var areaWidth = areaHeight * Camera.main.aspect;
            var bottomPos = -(areaHeight / 2f) - 0.5f;

            UpdateDisposable = Observable.EveryUpdate().Subscribe(_ =>
            {
                _Transform.position += Vector3.down * Time.deltaTime;

                _Transform.Rotate(Vector3.forward, 100f * Time.deltaTime);

                if (_Transform.position.y < bottomPos)
                {
                    ObjectPoolingManager.Kill(gameObject);
                }
            }).AddTo(this);
        }

        public void OnEndObject()
        {
            UpdateDisposable?.Dispose();
        }
    }
}