using System;
using System.Linq;
using Draft.Ammo;
using Modules.Utilities;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


    public abstract class CharacterBase : MonoBehaviour
    {
        [SerializeField] private float m_HealthPower = 100;
        [SerializeField] protected bool m_Immortal;
        [SerializeField] protected CharacterType m_CharacterType;
        private Action<CharacterBase> OnTerminatedAction;

        [SerializeField] private GameObject m_TerminateFxPrefab;
        [SerializeField] private float m_TerminateFxSize = 1;
        public Transform m_Transform { get; private set; }

        public float Width { private set; get; }
        public float Height { private set; get; }

        public enum CharacterType
        {
            None,
            Player,
            Enemy
        }

        protected virtual void Awake()
        {
            m_Transform = transform;
            if (m_HealthPower <= 0) m_HealthPower = 1;
            var poly = GetComponentInChildren<PolygonCollider2D>();
            var pointXs = poly.points.Select(_ => _.x).ToArray();
            var pointYs = poly.points.Select(_ => _.y).ToArray();

            Width = pointXs.Max() - pointXs.Min();
            Height = pointYs.Max() - pointYs.Min();

            poly.OnTriggerEnter2DAsObservable().Subscribe(_col =>
            {
                var ammo = _col.gameObject.GetComponent<AmmoBase>();
                if (ammo != null)
                {
                    if (ammo.GetShooterType() == CharacterType.Player && m_CharacterType == CharacterType.Enemy ||
                        ammo.GetShooterType() == CharacterType.Enemy && m_CharacterType == CharacterType.Player)
                    {

                        ammo.Terminate();
                        if (!m_Immortal)
                            ReductHealthPower(ammo.m_Damage);
                    }


                }

                var characterBase = _col.gameObject.GetComponent<CharacterBase>();
                if (characterBase != null && m_CharacterType == CharacterType.Player &&
                    characterBase.m_CharacterType == CharacterType.Enemy)
                {
                    //when player hit enemy
                    if (!m_Immortal)
                        ReductHealthPower(m_HealthPower);
                }
            }).AddTo(this);
        }

        protected virtual void Start()
        {
        }

        protected virtual void Terminate()
        {
            m_HealthPower = 0;
            CreateTerminateFx(m_Transform);
        }

        protected void CreateTerminateFx(Transform _transform)
        {
            AudioManager.PlayFX("explosion");

            var go = ObjectPoolingManager.CreateObject("terminate_fx", m_TerminateFxPrefab, _transform.position,
                _transform.rotation);
            go.transform.localScale = Vector3.one * m_TerminateFxSize;
        }

        public void ReductHealthPower(float _damage)
        {
            m_HealthPower -= _damage;
            if (m_HealthPower <= 0)
            {
                Terminate();
                OnTerminatedAction?.Invoke(this);
            }
        }

        public void SetHealthPower(float _hp)
        {
            m_HealthPower = _hp;
        }

        public float GetHealthPower() => m_HealthPower;

        public bool IsAlive()
        {
            return m_HealthPower > 0;
        }

        public bool IsOnScreen()
        {
            var areaHeight = 2f * Camera.main.orthographicSize;
            var areaWidth = areaHeight * Camera.main.aspect;
            var topPos = (areaHeight / 2f) + Height / 2f;
            var bottomPos = -(areaHeight / 2f) - Height / 2f;
            var leftPos = -(areaWidth / 2f) - Width / 2f;
            var rightPos = (areaWidth / 2f) + Width / 2f;

            var position = m_Transform.position;
            return position.x > leftPos && position.x < rightPos && position.y > bottomPos && position.y < topPos;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {

        }

        public IObservable<CharacterBase> OnTerminatedAsObservable()
        {
            return Observable.FromEvent<CharacterBase>(_e => OnTerminatedAction += _e, _e => OnTerminatedAction -= _e);
        }
    }
