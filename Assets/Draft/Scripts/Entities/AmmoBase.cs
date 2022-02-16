using System;
using Draft.Controller;
using UniRx;
using UnityEngine;


    public abstract class  AmmoBase : MonoBehaviour
    {

        [SerializeField] public float m_Damage = 100f;
        [SerializeField] public int m_ScoreAdd = 5;
        [SerializeField] protected CharacterBase.CharacterType m_Shooter = CharacterBase.CharacterType.None;
        [SerializeField] protected float m_Speed = 300f;
        [SerializeField] private GameObject m_TerminateFxPrefab;
        [SerializeField] private float m_TerminateFxSize = 1;
        protected Rigidbody2D m_Rigidbody2D;
        protected Transform _Transform;
        private Action<GameObject> OnTerminate;

        protected virtual void Awake()
        {
            _Transform = transform;
            m_Rigidbody2D = GetComponent<Rigidbody2D>();

        }

        void Start()
        {

        }

        protected virtual void FixedUpdate()
        {
            if (!IsOnScreen(m_Rigidbody2D.position))
            {
                OnTerminate?.Invoke(gameObject);
                gameObject.SetActive(false);
                m_Shooter = CharacterBase.CharacterType.None;
            }
        }


        public void SetShooter(CharacterBase.CharacterType shooter)
        {
            m_Shooter = shooter;
        }

        public CharacterBase.CharacterType GetShooterType()
        {
            return m_Shooter;
        }

        protected void CreateTerminateFx(Transform _transform)
        {
            var go = ObjectPoolingManager.CreateObject("terminate_fx", m_TerminateFxPrefab, _transform.position,
                _transform.rotation);
            go.transform.localScale = Vector3.one * m_TerminateFxSize;
        }

        public void Terminate()
        {

            CreateTerminateFx(_Transform);
            if (m_Shooter == CharacterBase.CharacterType.Player) PlayerController.Instance.AddScoreValue(m_ScoreAdd);
            OnTerminate?.Invoke(gameObject);
            m_Shooter = CharacterBase.CharacterType.None;
        }

        private bool IsOnScreen(Vector2 _input)
        {
            var areaHeight = 2f * Camera.main.orthographicSize;
            var areaWidth = areaHeight * Camera.main.aspect;
            var topPos = (areaHeight / 2f);
            var bottomPos = -(areaHeight / 2f);
            var leftPos = -(areaWidth / 2f);
            var rightPos = (areaWidth / 2f);

            return _input.x > leftPos && _input.x < rightPos && _input.y > bottomPos && _input.y < topPos;
        }

        public IObservable<GameObject> OnTerminateAsObservable()
        {
            return Observable.FromEvent<GameObject>(_e => OnTerminate += _e, _e => OnTerminate -= _e);
        }


    }

