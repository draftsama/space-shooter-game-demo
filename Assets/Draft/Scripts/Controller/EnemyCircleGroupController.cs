using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Utilities;
using UniRx;
using UnityEngine;

namespace Draft.Controller
{
    public class EnemyCircleGroupController : GameEventBase
    {
        
        [SerializeField] private GameObject m_Prefab;
        [SerializeField] private float m_Health = 3000f;
        [SerializeField] private float m_Radius = 2f;
        [SerializeField] private int m_Max = 6;
        [SerializeField] private float m_Speed = 16f;
        [SerializeField] private MovementQueueController m_MovementQueueController;
        [SerializeField] private bool m_LoopMovement = false;

        private float _Axis = 0;
        private float _AnglePart = 0;
        private Transform _Transform;

        private List<EnemyController> m_EnemyList;


        void Start()
        {


            _Transform = transform;
            InitEnemy();

            m_MovementQueueController.OnMovementUpdateStatus().Subscribe(_response =>
            {
                //  Debug.Log($"response > index:{_response.m_index} - status:{_response.m_Status}");

                //enemy shoting manager
                if (_response.m_Status == MovementQueueController.Status.End)
                {
                    if (_response.m_index == 1 || _response.m_index == 3)
                        ShootPattern1(_loopTarget: 2);
                    else if (_response.m_index == 5)
                    {
                        ShootPattern1(0.25f, 3);
                        SpecialMove1(0.25f);
                    }
                }



            }).AddTo(this);

            m_MovementQueueController.OnEndMovement().Subscribe(_ => StopEvent()).AddTo(this);
        }

        private void InitEnemy()
        {
            _AnglePart = 360f / m_Max;

            m_EnemyList = new List<EnemyController>(m_Max);
            for (int i = 0; i < m_Max; i++)
            {
                var rot = Quaternion.Euler(0, 0, (_AnglePart * i) + _Axis);

                var go = ObjectPoolingManager.CreateObject("enemy", m_Prefab, Vector3.zero, Quaternion.identity, null);

                go.transform.position = rot * new Vector3(0, m_Radius, 0) + _Transform.position;
                go.name = $"enemy{i}";
                var enemy = go.GetComponent<EnemyController>();
                enemy.SetHealthPower(m_Health);
                enemy.OnTerminatedAsObservable().Subscribe(_e => { _e.gameObject.SetActive(false); }).AddTo(enemy);
                m_EnemyList.Add(enemy);
            }

            foreach (var enemy in m_EnemyList)
            {
                enemy.gameObject.SetActive(false);
            }
        }

        private void RelifeEnemy()
        {
            foreach (var enemy in m_EnemyList)
            {
                enemy.SetHealthPower(m_Health);
                enemy.gameObject.SetActive(true);
            }
        }

        private void EnemyMovementRunning()
        {

            if (m_EnemyList.FirstOrDefault(_ => _.IsAlive()) == null)
            {
                //when no more enemy is alive 

                StopEvent();

            }

            _Axis += Time.deltaTime * m_Speed;
            if (_Axis >= 360) _Axis = 0;
            for (int i = 0; i < m_EnemyList.Count; i++)
            {
                var enemy = m_EnemyList[i];
                if (enemy == null)
                    continue;


                var angle = (_AnglePart * i) + _Axis;
                var rot = Quaternion.Euler(0, 0, angle);
                enemy.m_Transform.position = rot * new Vector3(0, m_Radius, 0) + _Transform.position;
            }
        }

        public void SpecialMove1(float _delay = 0)
        {
            Observable.Timer(TimeSpan.FromMilliseconds(_delay * 1000)).Subscribe(_ =>
            {
                UpRadius();
                Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(_ => { RobackRadius(); }).AddTo(this);
            }).AddTo(this);

        }

        private IDisposable _ShootDisposable;

        public void ShootPattern1(float _delay = 0, int _loopTarget = 1, float repeatTime = 0.025f)
        {
            _ShootDisposable?.Dispose();
            var index = 0;
            var countDown = _delay;
            var loopCount = 0;

            _ShootDisposable = Observable.EveryUpdate().Subscribe(_ =>
            {
                if (m_EnemyList.Count == 0) _ShootDisposable?.Dispose();

                countDown -= Time.deltaTime;

                if (countDown <= 0)
                {
                    _Axis += Time.deltaTime * m_Speed * 2f;

                    if (index >= m_EnemyList.Count)
                    {
                        index = 0;
                        countDown = repeatTime;
                        loopCount++;
                        if (loopCount >= _loopTarget)
                        {
                            _ShootDisposable?.Dispose();
                            return;
                        }
                    }
                    else
                    {
                        countDown = repeatTime;
                    }

                    if (m_EnemyList.Count == 0)
                    {
                        _ShootDisposable?.Dispose();
                        return;
                    }

                    m_EnemyList[index].Shoot();
                    index++;
                }
            }).AddTo(this);
        }

        private IDisposable _ScaleAnimationDisposable;
        private float _OldRadius;

        public void UpRadius()
        {
            _OldRadius = m_Radius;
            _ScaleAnimationDisposable?.Dispose();
            _ScaleAnimationDisposable = LerpThread.FloatLerp(500, m_Radius, _OldRadius + 1)
                .Subscribe(_ => { m_Radius = _; }).AddTo(this);
        }

        public void RobackRadius()
        {
            _ScaleAnimationDisposable?.Dispose();
            _ScaleAnimationDisposable = LerpThread.FloatLerp(500, m_Radius, _OldRadius)
                .Subscribe(_ => { m_Radius = _; })
                .AddTo(this);
        }

        private CompositeDisposable _CompositeDisposable;
        private GameEventBase m_GameEventBaseImplementation;

        public override void StartEvent()
        {
            _CompositeDisposable?.Dispose();
            _CompositeDisposable = new CompositeDisposable();
            _CompositeDisposable.Add(m_MovementQueueController.PlayMovement(m_LoopMovement).AddTo(this));
            RelifeEnemy();
            _CompositeDisposable.Add(Observable.EveryUpdate().Subscribe(_ => EnemyMovementRunning()).AddTo(this));
        }

        public override void StopEvent()
        {
            base.StopEvent();

            m_MovementQueueController.ResetPosition();
            foreach (var enemy in m_EnemyList)
            {
                if (enemy == null) continue;
                ;
                enemy.transform.position = _Transform.position;
                enemy.gameObject.SetActive(false);
            }

            _CompositeDisposable?.Dispose();

        }


    }
}