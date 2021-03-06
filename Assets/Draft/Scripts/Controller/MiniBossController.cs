using UniRx;
using UnityEngine;

namespace Draft.Controller
{
    public class MiniBossController : GameEventBase
    {
        [SerializeField] private GameObject m_Prefab;
        [SerializeField] private float m_HealthPower;
        [SerializeField] private MovementQueueController m_MovementQueueController;

        private Transform _Transform;
        private GameObject _EnemyObj;

        void Start()
        {
            _Transform = transform;
        }

        private CompositeDisposable _CompositeDisposable;

        public override void StartEvent()
        {
            base.StartEvent();
            _CompositeDisposable?.Dispose();
            _CompositeDisposable = new CompositeDisposable();
            var instanName = GetInstanceID().ToString();
            _EnemyObj = ObjectPoolingManager.CreateObject($"{instanName}_enemy", m_Prefab, _Transform);
            _EnemyObj.transform.localPosition = Vector3.zero;
            var enemy = _EnemyObj.GetComponent<EnemyController>();
            enemy.SetHealthPower(m_HealthPower);
            enemy.OnTerminatedAsObservable().Subscribe(_ =>
            {
                _EnemyObj.SetActive(false);
                StopEvent();
            }).AddTo(this);
            _CompositeDisposable.Add(Observable.EveryUpdate().Subscribe(_ => { enemy.Shoot(); }).AddTo(this));
            _CompositeDisposable.Add(m_MovementQueueController.PlayMovement(true).AddTo(this));
        }


        public override void StopEvent()
        {
            base.StopEvent();
            _CompositeDisposable?.Dispose();

        }
    }
}