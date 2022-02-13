using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modules.Utilities;
using UniRx;
using UnityEngine;

public class EnemyFollowGroupController : GameEventBase
{
    [SerializeField] private GameObject m_Prefab;
    [SerializeField] private float m_HealthPower = 3000f;
    [SerializeField] private float m_Speed = 1.5f;
    [SerializeField] private int m_Max = 5;

    private List<EnemyController> m_EnemyList;
    private Transform _Transform;
    [SerializeField] private MovementQueueController m_MovementQueueController;
    [SerializeField] private bool m_LoopMovement = true;

    void Start()
    {
        _Transform = transform;
        m_MovementQueueController.OnMovementUpdateStatus().Subscribe(_response =>
        {
            //  Debug.Log($"response > index:{_response.m_index} - status:{_response.m_Status}");

            //enemy shoting manager
            if (_response.m_Status == MovementQueueController.Status.Start)
            {
                if (_response.m_index == 1)
                    ShootAll(2.5f,2);
                else if (_response.m_index == 3)
                    ShootAll(2f, 4);
            }
        }).AddTo(this);
    }

    private CompositeDisposable _CompositeDisposable;

    public override void StartEvent()
    {
        base.StartEvent();
        m_EnemyList?.Clear();
        m_EnemyList = new List<EnemyController>(m_Max);
        for (int i = 0; i < m_Max; i++)
        {
            var go = ObjectPoolingManager.CreateObject(m_Prefab.name, m_Prefab, _Transform.position,
                _Transform.rotation);

            var enemy = go.GetComponent<EnemyController>();
            enemy.SetHealthPower(m_HealthPower);
            enemy.OnTerminatedAsObservable().Subscribe(_ => { _.gameObject.SetActive(false); }).AddTo(this);

            m_EnemyList.Add(enemy);
        }

        _CompositeDisposable?.Dispose();
        _CompositeDisposable = new CompositeDisposable();
        _CompositeDisposable.Add(m_MovementQueueController.PlayMovement(m_LoopMovement).AddTo(this));

        _CompositeDisposable.Add(Observable.EveryLateUpdate().Subscribe(_ =>
        {
            if (m_EnemyList.FirstOrDefault(_ => _.IsAlive()) == null)
            {
                //when no more enemy is alive 
                StopEvent();
            }

            
            for (int i = 0; i < m_Max; i++)
            {
                var enemy = m_EnemyList[i];

                if (i == 0)
                {
                    var position = enemy.m_Transform.position;
                    var dir = _Transform.position - position;
                    position += dir * m_Speed * Time.deltaTime;
                    enemy.m_Transform.position = position;
                }
                else
                {
                    var lead = m_EnemyList[i - 1];
                    var position = enemy.m_Transform.position;
                    var dir = lead.m_Transform.position - position;
                    position += dir * m_Speed * Time.deltaTime;
                    enemy.m_Transform.position = position;
                }
            }
        }).AddTo(this));
    }

    public override void StopEvent()
    {
        base.StopEvent();
        _CompositeDisposable?.Dispose();
    }

    private IDisposable _ShootDisposable;

    public void ShootAll(float _delay = 0, int _loopTarget = 1 , float repeatTime = 0.1f)
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

                if (PlayerController.Instance.IsAlive())
                {
                    var enemy = m_EnemyList[index];
                    var direction = (Vector2)PlayerController.Instance.m_Transform.position -
                                    (Vector2)enemy.m_Projectile.m_Transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    enemy.m_Projectile.m_Transform.rotation = Quaternion.Euler(0, 0, angle - 90);
                    enemy.Shoot();
                }

                index++;
            }
        }).AddTo(this);
    }
}